﻿using Mono.Data.Sqlite;
using SabreTools.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabreTools
{
	public partial class SabreTools
	{
		#region Init Methods

		/// <summary>
		/// Wrap sorting files using an input DAT
		/// </summary>
		/// <param name="inputs">List of all inputted files and folders</param>
		/// <param name="outDir">Output directory (empty for default directory)</param>
		/// <param name="tempDir">Temporary directory for archive extraction</param>
		/// <param name="delete">True if input files should be deleted, false otherwise</param>
		/// <param name="romba">True if files should be output in Romba depot folders, false otherwise</param>
		/// <param name="sevenzip">Integer representing the archive handling level for 7z</param>
		/// <param name="gz">Integer representing the archive handling level for GZip</param>
		/// <param name="rar">Integer representing the archive handling level for RAR</param>
		/// <param name="zip">Integer representing the archive handling level for Zip</param>
		/// <param name="logger">Logger object for file and console output</param>
		public static bool InitConvertFolderTGZ(List<string> inputs, string outDir, string tempDir, bool delete,
			bool romba, int sevenzip, int gz, int rar, int zip, Logger logger)
		{
			// Get all individual files from the inputs
			List<string> newinputs = new List<string>();
			foreach (string input in inputs)
			{
				if (File.Exists(input))
				{
					newinputs.Add(Path.GetFullPath(input));
				}
				else if (Directory.Exists(input))
				{
					foreach (string file in Directory.EnumerateFiles(input, "*", SearchOption.AllDirectories))
					{
						newinputs.Add(Path.GetFullPath(file));
					}
				}
			}

			SimpleSort ss = new SimpleSort(new DatFile(), newinputs, outDir, tempDir, false, false, false, delete, false, romba, sevenzip, gz, rar, zip, false, logger);
			return ss.Convert();
		}

		/// <summary>
		/// Wrap creating a DAT file from files or a directory in parallel
		/// </summary>
		/// <param name="inputs">List of input filenames</param>
		/// <param name="filename">New filename</param>
		/// <param name="name">New name</param>
		/// <param name="description">New description</param>
		/// <param name="category">New category</param>
		/// <param name="version">New version</param>
		/// <param name="author">New author</param>
		/// <param name="forceunpack">True to set forcepacking="unzip" on the created file, false otherwise</param>
		/// <param name="outputFormat">OutputFormat to be used for outputting the DAT</param>
		/// <param name="romba">True to enable reading a directory like a Romba depot, false otherwise</param>
		/// <param name="superdat">True to enable SuperDAT-style reading, false otherwise</param>
		/// <param name="noMD5">True to disable getting MD5 hash, false otherwise</param>
		/// <param name="noSHA1">True to disable getting SHA-1 hash, false otherwise</param>
		/// <param name="bare">True if the date should be omitted from the DAT, false otherwise</param>
		/// <param name="archivesAsFiles">True if archives should be treated as files, false otherwise</param>
		/// <param name="enableGzip">True if GZIP archives should be treated as files, false otherwise</param>
		/// <param name="addBlanks">True if blank items should be created for empty folders, false otherwise</param>
		/// <param name="addDate">True if dates should be archived for all files, false otherwise</param>
		/// <param name="tempDir">Name of the directory to create a temp folder in (blank is current directory</param>
		/// <param name="copyFiles">True if files should be copied to the temp directory before hashing, false otherwise</param>
		/// <param name="maxDegreeOfParallelism">Integer representing the maximum amount of parallelization to be used</param>
		private static void InitDatFromDir(List<string> inputs,
			string filename,
			string name,
			string description,
			string category,
			string version,
			string author,
			bool forceunpack,
			OutputFormat outputFormat,
			bool romba,
			bool superdat,
			bool noMD5,
			bool noSHA1,
			bool bare,
			bool archivesAsFiles,
			bool enableGzip,
			bool addBlanks,
			bool addDate,
			string tempDir,
			bool copyFiles,
			int maxDegreeOfParallelism)
		{
			// Create a new DATFromDir object and process the inputs
			DatFile basedat = new DatFile
			{
				FileName = filename,
				Name = name,
				Description = description,
				Category = category,
				Version = version,
				Date = DateTime.Now.ToString("yyyy-MM-dd"),
				Author = author,
				ForcePacking = (forceunpack ? ForcePacking.Unzip : ForcePacking.None),
				OutputFormat = (outputFormat == 0 ? OutputFormat.Xml : outputFormat),
				Romba = romba,
				Type = (superdat ? "SuperDAT" : ""),
				Files = new SortedDictionary<string, List<DatItem>>(),
			};

			// For each input directory, create a DAT
			foreach (string path in inputs)
			{
				if (Directory.Exists(path))
				{
					// Clone the base Dat for information
					DatFile datdata = (DatFile)basedat.Clone();
					datdata.Files = new SortedDictionary<string, List<DatItem>>();

					string basePath = Path.GetFullPath(path);
					bool success = datdata.PopulateDatFromDir(basePath, noMD5, noSHA1, bare, archivesAsFiles, enableGzip, addBlanks, addDate, tempDir, copyFiles, maxDegreeOfParallelism, _logger);

					// If it was a success, write the DAT out
					if (success)
					{
						datdata.WriteToFile("", _logger);
					}

					// Otherwise, show the help
					else
					{
						Console.WriteLine();
						Build.Help();
					}
				}
			}
		}

		/// <summary>
		/// Wrap splitting a DAT by 2 extensions
		/// </summary>
		/// <param name="inputs">Input files or folders to be split</param>
		/// <param name="exta">First extension to split on</param>
		/// <param name="extb">Second extension to split on</param>
		/// <param name="outDir">Output directory for the split files</param>
		private static void InitExtSplit(List<string> inputs, string exta, string extb, string outDir)
		{
			// Convert comma-separated strings to list
			List<string> extaList = exta.Split(',').ToList();
			List<string> extbList = extb.Split(',').ToList();

			// Loop over the input files
			foreach (string input in inputs)
			{
				if (File.Exists(input))
				{
					DatFile.SplitByExt(Path.GetFullPath(input), outDir, Path.GetDirectoryName(input), extaList, extbList, _logger);
				}
				else if (Directory.Exists(input))
				{
					foreach (string file in Directory.EnumerateFiles(input, "*", SearchOption.AllDirectories))
					{
						DatFile.SplitByExt(file, outDir, (input.EndsWith(Path.DirectorySeparatorChar.ToString()) ? input : input + Path.DirectorySeparatorChar), extaList, extbList, _logger);
					}
				}
				else
				{
					_logger.Error(input + " is not a valid file or folder!");
					Console.WriteLine();
					Build.Help();
					return;
				}
			}
		}

		/// <summary>
		/// Wrap splitting a DAT by best available hashes
		/// </summary>
		/// <param name="inputs">List of inputs to be used</param>
		/// <param name="outDir">Output directory for the split files</param>
		private static void InitHashSplit(List<string> inputs, string outDir)
		{
			// Loop over the input files
			foreach (string input in inputs)
			{
				if (File.Exists(input))
				{
					DatFile.SplitByHash(Path.GetFullPath(input), outDir, Path.GetDirectoryName(input), _logger);
				}
				else if (Directory.Exists(input))
				{
					foreach (string file in Directory.EnumerateFiles(input, "*", SearchOption.AllDirectories))
					{
						DatFile.SplitByHash(file, outDir, (input.EndsWith(Path.DirectorySeparatorChar.ToString()) ? input : input + Path.DirectorySeparatorChar), _logger);
					}
				}
				else
				{
					_logger.Error(input + " is not a valid file or folder!");
					Console.WriteLine();
					Build.Help();
					return;
				}
			}
		}

		/// <summary>
		/// Wrap extracting and replacing headers
		/// </summary>
		/// <param name="inputs">Input file or folder names</param>
		/// <param name="restore">False if we're extracting headers (default), true if we're restoring them</param>
		/// <param name="outDir">Output directory to write new files to, blank defaults to rom folder</param>
		/// <param name="logger">Logger object for file and console output</param>
		private static void InitHeaderer(List<string> inputs, bool restore, string outDir, Logger logger)
		{
			foreach (string input in inputs)
			{
				if (File.Exists(input))
				{
					if (restore)
					{
						FileTools.RestoreHeader(input, outDir, logger);
					}
					else
					{
						FileTools.DetectSkipperAndTransform(input, outDir, logger);
					}
				}
				else if (Directory.Exists(input))
				{
					foreach (string sub in Directory.EnumerateFiles(input, "*", SearchOption.AllDirectories))
					{
						if (restore)
						{
							FileTools.RestoreHeader(sub, outDir, logger);
						}
						else
						{
							FileTools.DetectSkipperAndTransform(sub, outDir, logger);
						}
					}
				}
			}
		}

		/// <summary>
		/// Wrap sorting files using an input DAT
		/// </summary>
		/// <param name="datfiles">Names of the DATs to compare against</param>
		/// <param name="inputs">List of input files/folders to check</param>
		/// <param name="outDir">Output directory to use to build to</param>
		/// <param name="tempDir">Temporary directory for archive extraction</param>
		/// <param name="quickScan">True to enable external scanning of archives, false otherwise</param>
		/// <param name="sevenzip">Integer representing the archive handling level for 7z</param>
		/// <param name="toFolder">True if files should be output to folder, false otherwise</param>
		/// <param name="verify">True if output directory should be checked instead of rebuilt to, false otherwise</param>
		/// <param name="delete">True if input files should be deleted, false otherwise</param>
		/// <param name="torrentX">True is for TorrentZip, False is for TorrentGZ, Null is for standard zip</param>
		/// <param name="romba">True if files should be output in Romba depot folders, false otherwise</param>
		/// <param name="gz">Integer representing the archive handling level for GZip</param>
		/// <param name="rar">Integer representing the archive handling level for RAR</param>
		/// <param name="zip">Integer representing the archive handling level for Zip</param>
		/// <param name="updateDat">True if the updated DAT should be output, false otherwise</param>
		/// <param name="logger">Logger object for file and console output</param>
		private static void InitSortVerify(List<string> datfiles, List<string> inputs, string outDir, string tempDir, bool quickScan,
			bool toFolder, bool verify, bool delete, bool? torrentX, bool romba, int sevenzip, int gz, int rar, int zip, bool updateDat, Logger logger)
		{
			// Add all of the input DATs into one huge internal DAT
			DatFile datdata = new DatFile();
			foreach (string datfile in datfiles)
			{
				datdata.Parse(datfile, 99, 99, logger);
			}

			SimpleSort ss = new SimpleSort(datdata, inputs, outDir, tempDir, quickScan, toFolder, verify,
				delete, torrentX, romba, sevenzip, gz, rar, zip, updateDat, logger);
			ss.StartProcessing();
		}

		/// <summary>
		/// Wrap getting statistics on a DAT or folder of DATs
		/// </summary>
		/// <param name="inputs">List of inputs to be used</param>
		/// <param name="single">True to show individual DAT statistics, false otherwise</param>
		private static void InitStats(List<string> inputs, bool single)
		{
			Logger statlog = new Logger(true, "stats.txt");
			DatFile.OutputStats(inputs, single, statlog);
			statlog.Close(true);
		}

		/// <summary>
		/// Wrap splitting a DAT by item type
		/// </summary>
		/// <param name="inputs">List of inputs to be used</param>
		/// <param name="outDir">Output directory for the split files</param>
		private static void InitTypeSplit(List<string> inputs, string outDir)
		{
			// Loop over the input files
			foreach (string input in inputs)
			{
				if (File.Exists(input))
				{
					DatFile.SplitByType(Path.GetFullPath(input), outDir, Path.GetFullPath(Path.GetDirectoryName(input)), _logger);
				}
				else if (Directory.Exists(input))
				{
					foreach (string file in Directory.EnumerateFiles(input, "*", SearchOption.AllDirectories))
					{
						DatFile.SplitByType(file, outDir, Path.GetFullPath((input.EndsWith(Path.DirectorySeparatorChar.ToString()) ? input : input + Path.DirectorySeparatorChar)), _logger);
					}
				}
				else
				{
					_logger.Error(input + " is not a valid file or folder!");
					Console.WriteLine();
					Build.Help();
					return;
				}
			}
		}

		/// <summary>
		/// Wrap converting and updating DAT file from any format to any format
		/// </summary>
		/// <param name="input">List of input filenames</param>
		/// /* Normal DAT header info */
		/// <param name="filename">New filename</param>
		/// <param name="name">New name</param>
		/// <param name="description">New description</param>
		/// <param name="rootdir">New rootdir</param>
		/// <param name="category">New category</param>
		/// <param name="version">New version</param>
		/// <param name="date">New date</param>
		/// <param name="author">New author</param>
		/// <param name="email">New email</param>
		/// <param name="homepage">New homepage</param>
		/// <param name="url">New URL</param>
		/// <param name="comment">New comment</param>
		/// <param name="header">New header</param>
		/// <param name="superdat">True to set SuperDAT type, false otherwise</param>
		/// <param name="forcemerge">None, Split, Full</param>
		/// <param name="forcend">None, Obsolete, Required, Ignore</param>
		/// <param name="forcepack">None, Zip, Unzip</param>
		/// <param name="outputFormat">Non-zero flag for output format, zero otherwise for default</param>
		/// /* Missfile-specific DAT info */
		/// <param name="usegame">True if games are to be used in output, false if roms are</param>
		/// <param name="prefix">Generic prefix to be added to each line</param>
		/// <param name="postfix">Generic postfix to be added to each line</param>
		/// <param name="quotes">Add quotes to each item</param>
		/// <param name="repext">Replace all extensions with another</param>
		/// <param name="addext">Add an extension to all items</param>
		/// <param name="remext">Remove all extensions</param>
		/// <param name="datprefix">Add the dat name as a directory prefix</param>
		/// <param name="romba">Output files in romba format</param>
		/// <param name="tsv">True to output files in TSV format, false to output files in CSV format, null otherwise</param>
		/// /* Merging and Diffing info */
		/// <param name="merge">True if input files should be merged into a single file, false otherwise</param>
		/// <param name="diffMode">Non-zero flag for diffing mode, zero otherwise</param>
		/// <param name="cascade">True if the diffed files should be cascade diffed, false if diffed files should be reverse cascaded, null otherwise</param>
		/// <param name="inplace">True if the cascade-diffed files should overwrite their inputs, false otherwise</param>
		/// <param name="skip">True if the first cascaded diff file should be skipped on output, false otherwise</param>
		/// <param name="bare">True if the date should not be appended to the default name, false otherwise [OBSOLETE]</param>
		/// /* Filtering info */
		/// <param name="gamename">Name of the game to match (can use asterisk-partials)</param>
		/// <param name="romname">Name of the rom to match (can use asterisk-partials)</param>
		/// <param name="romtype">Type of the rom to match</param>
		/// <param name="sgt">Find roms greater than or equal to this size</param>
		/// <param name="slt">Find roms less than or equal to this size</param>
		/// <param name="seq">Find roms equal to this size</param>
		/// <param name="crc">CRC of the rom to match (can use asterisk-partials)</param>
		/// <param name="md5">MD5 of the rom to match (can use asterisk-partials)</param>
		/// <param name="sha1">SHA-1 of the rom to match (can use asterisk-partials)</param>
		/// <param name="status">Select roms with the given item status</param>
		/// /* Trimming info */
		/// <param name="trim">True if we are supposed to trim names to NTFS length, false otherwise</param>
		/// <param name="single">True if all games should be replaced by '!', false otherwise</param>
		/// <param name="root">String representing root directory to compare against for length calculation</param>
		/// /* Output DAT info */
		/// <param name="outDir">Optional param for output directory</param>
		/// <param name="clean">True to clean the game names to WoD standard, false otherwise (default)</param>
		/// <param name="softlist">True to allow SL DATs to have game names used instead of descriptions, false otherwise (default)</param>
		/// <param name="dedup">True to dedupe the roms in the DAT, false otherwise (default)</param>
		/// /* Multithreading info */
		/// <param name="maxDegreeOfParallelism">Integer representing the maximum amount of parallelization to be used</param>
		private static void InitUpdate(List<string> inputs,
			/* Normal DAT header info */
			string filename,
			string name,
			string description,
			string rootdir,
			string category,
			string version,
			string date,
			string author,
			string email,
			string homepage,
			string url,
			string comment,
			string header,
			bool superdat,
			string forcemerge,
			string forcend,
			string forcepack,
			OutputFormat outputFormat,

			/* Missfile-specific DAT info */
			bool usegame,
			string prefix,
			string postfix,
			bool quotes,
			string repext,
			string addext,
			bool remext,
			bool datprefix,
			bool romba,
			bool? tsv,

			/* Merging and Diffing info */
			bool merge,
			DiffMode diffMode,
			bool? cascade,
			bool inplace,
			bool skip,
			bool bare,

			/* Filtering info */
			string gamename,
			string romname,
			string romtype,
			long sgt,
			long slt,
			long seq,
			string crc,
			string md5,
			string sha1,
			string status,

			/* Trimming info */
			bool trim,
			bool single,
			string root,

			/* Output DAT info */
			string outDir,
			bool clean,
			bool softlist,
			bool dedup,
			
			/* Multithreading info */
			int maxDegreeOfParallelism)
		{
			// Set the special flags
			ForceMerging fm = ForceMerging.None;
			switch (forcemerge.ToLowerInvariant())
			{
				case "none":
				default:
					fm = ForceMerging.None;
					break;
				case "split":
					fm = ForceMerging.Split;
					break;
				case "full":
					fm = ForceMerging.Full;
					break;
			}

			ForceNodump fn = ForceNodump.None;
			switch (forcend.ToLowerInvariant())
			{
				case "none":
				default:
					fn = ForceNodump.None;
					break;
				case "obsolete":
					fn = ForceNodump.Obsolete;
					break;
				case "required":
					fn = ForceNodump.Required;
					break;
				case "ignore":
					fn = ForceNodump.Ignore;
					break;
			}

			ForcePacking fp = ForcePacking.None;
			switch (forcepack.ToLowerInvariant())
			{
				case "none":
				default:
					fp = ForcePacking.None;
					break;
				case "zip":
					fp = ForcePacking.Zip;
					break;
				case "unzip":
					fp = ForcePacking.Unzip;
					break;
			}

			// Set the status flag for filtering
			ItemStatus itemStatus = ItemStatus.NULL;
			switch(status.ToLowerInvariant())
			{
				case "none":
					itemStatus = ItemStatus.None;
					break;
				case "good":
					itemStatus = ItemStatus.Good;
					break;
				case "baddump":
					itemStatus = ItemStatus.BadDump;
					break;
				case "nodump":
					itemStatus = ItemStatus.Nodump;
					break;
				case "verified":
					itemStatus = ItemStatus.Verified;
					break;
				case "notnodump":
					itemStatus = ItemStatus.NotNodump;
					break;
			}

			// Normalize the extensions
			addext = (addext == "" || addext.StartsWith(".") ? addext : "." + addext);
			repext = (repext == "" || repext.StartsWith(".") ? repext : "." + repext);

			// If we're in merge or diff mode and the names aren't set, set defaults
			if (merge || diffMode != 0)
			{
				// Get the values that will be used
				if (date == "")
				{
					date = DateTime.Now.ToString("yyyy-MM-dd");
				}
				if (name == "")
				{
					name = (diffMode != 0 ? "DiffDAT" : "MergeDAT") + (superdat ? "-SuperDAT" : "") + (dedup ? "-deduped" : "");
				}
				if (description == "")
				{
					description = (diffMode != 0 ? "DiffDAT" : "MergeDAT") + (superdat ? "-SuperDAT" : "") + (dedup ? " - deduped" : "");
					if (!bare)
					{
						description += " (" + date + ")";
					}
				}
				if (category == "" && diffMode != 0)
				{
					category = "DiffDAT";
				}
				if (author == "")
				{
					author = "SabreTools";
				}
			}

			// Populate the DatData object
			DatFile userInputDat = new DatFile
			{
				FileName = filename,
				Name = name,
				Description = description,
				RootDir = rootdir,
				Category = category,
				Version = version,
				Date = date,
				Author = author,
				Email = email,
				Homepage = homepage,
				Url = url,
				Comment = comment,
				Header = header,
				Type = (superdat ? "SuperDAT" : null),
				ForceMerging = fm,
				ForceNodump = fn,
				ForcePacking = fp,
				MergeRoms = dedup,
				OutputFormat = outputFormat,

				UseGame = usegame,
				Prefix = prefix,
				Postfix = postfix,
				Quotes = quotes,
				RepExt = repext,
				AddExt = addext,
				RemExt = remext,
				GameName = datprefix,
				Romba = romba,
				XSV = tsv,
			};

			userInputDat.Update(inputs, outDir, merge, diffMode, cascade, inplace, skip, bare, clean, softlist,
				gamename, romname, romtype, sgt, slt, seq, crc, md5, sha1, itemStatus, trim, single, root, maxDegreeOfParallelism, _logger);
		}

		#endregion
	}
}
