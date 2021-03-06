﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using SabreTools.Library.DatItems;
using SabreTools.Library.IO;

namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Represents parsing and writing of an Everdrive SMDB file
    /// </summary>
    internal class EverdriveSMDB : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public EverdriveSMDB(DatFile datFile)
            : base(datFile)
        {
        }

        /// <summary>
        /// Parse an Everdrive SMDB file and return all found games within
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        protected override void ParseFile(string filename, int indexId, bool keep, bool throwOnError = false)
        {
            // Open a file reader
            Encoding enc = FileExtensions.GetEncoding(filename);
            SeparatedValueReader svr = new SeparatedValueReader(FileExtensions.TryOpenRead(filename), enc)
            {
                Header = false,
                Quotes = false,
                Separator = '\t',
                VerifyFieldCount = false,
            };

            while (!svr.EndOfStream)
            {
                try
                {
                    // If we can't read the next line, break
                    if (!svr.ReadNextLine())
                        break;

                    // If the line returns null somehow, skip
                    if (svr.Line == null)
                        continue;

                    /*
                    The gameinfo order is as follows
                    0 - SHA-256
                    1 - Machine Name/Filename
                    2 - SHA-1
                    3 - MD5
                    4 - CRC32
                    */

                    string[] fullname = svr.Line[1].Split('/');

                    Rom rom = new Rom
                    {
                        Name = svr.Line[1].Substring(fullname[0].Length + 1),
                        Size = null, // No size provided, but we don't want the size being 0
                        CRC = svr.Line[4],
                        MD5 = svr.Line[3],
                        SHA1 = svr.Line[2],
                        SHA256 = svr.Line[0],
                        ItemStatus = ItemStatus.None,

                        Machine = new Machine
                        {
                            Name = fullname[0],
                            Description = fullname[0],
                        },

                        Source = new Source
                        {
                            Index = indexId,
                            Name = filename,
                        },
                    };

                    // Now process and add the rom
                    ParseAddHelper(rom);
                }
                catch (Exception ex)
                {
                    string message = $"'{filename}' - There was an error parsing line {svr.LineNumber} '{svr.CurrentLine}'";
                    logger.Error(ex, message);
                    if (throwOnError)
                    {
                        svr.Dispose();
                        throw new Exception(message, ex);
                    }
                }
            }

            svr.Dispose();
        }

        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[] { ItemType.Rom };
        }

        /// <summary>
        /// Create and open an output file for writing direct from a dictionary
        /// </summary>
        /// <param name="outfile">Name of the file to write to</param>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DAT was written correctly, false otherwise</returns>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Opening file for writing: {outfile}");
                FileStream fs = FileExtensions.TryCreate(outfile);

                // If we get back null for some reason, just log and return
                if (fs == null)
                {
                    logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                SeparatedValueWriter svw = new SeparatedValueWriter(fs, new UTF8Encoding(false))
                {
                    Quotes = false,
                    Separator = '\t',
                    VerifyFieldCount = true
                };

                // Use a sorted list of games to output
                foreach (string key in Items.SortedKeys)
                {
                    List<DatItem> datItems = Items.FilteredItems(key);

                    // If this machine doesn't contain any writable items, skip
                    if (!ContainsWritable(datItems))
                        continue;

                    // Resolve the names in the block
                    datItems = DatItem.ResolveNames(datItems);

                    for (int index = 0; index < datItems.Count; index++)
                    {
                        DatItem datItem = datItems[index];

                        // Check for a "null" item
                        datItem = ProcessNullifiedItem(datItem);

                        // Write out the item if we're not ignoring
                        if (!ShouldIgnore(datItem, ignoreblanks))
                            WriteDatItem(svw, datItem);
                    }
                }

                logger.Verbose($"File written!{Environment.NewLine}");
                svw.Dispose();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                if (throwOnError) throw ex;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="svw">SeparatedValueWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        private void WriteDatItem(SeparatedValueWriter svw, DatItem datItem)
        {
            // No game should start with a path separator
            datItem.Machine.Name = datItem.Machine.Name.TrimStart(Path.DirectorySeparatorChar);

            // Pre-process the item name
            ProcessItemName(datItem, true);

            // Build the state
            switch (datItem.ItemType)
            {
                case ItemType.Rom:
                    var rom = datItem as Rom;

                    string[] fields = new string[]
                    {
                            rom.SHA256 ?? string.Empty,
                            $"{rom.Machine.Name ?? string.Empty}/",
                            rom.Name ?? string.Empty,
                            rom.SHA1 ?? string.Empty,
                            rom.MD5 ?? string.Empty,
                            rom.CRC ?? string.Empty,
                    };

                    svw.WriteValues(fields);

                    break;
            }

            svw.Flush();
        }
    }
}
