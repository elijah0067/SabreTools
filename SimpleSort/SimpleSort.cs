﻿using SabreTools.Helper;
using SharpCompress.Archive;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace SabreTools
{
	public class SimpleSort
	{
		// Private instance variables
		private DatData _datdata;
		private List<string> _inputs;
		private string _outdir;
		private string _tempdir;
		private bool _externalScan;
		private ArchiveScanLevel _7z;
		private ArchiveScanLevel _gz;
		private ArchiveScanLevel _rar;
		private ArchiveScanLevel _zip;
		private Logger _logger;

		/// <summary>
		/// Create a new SimpleSort object
		/// </summary>
		/// <param name="datdata">Name of the DAT to compare against</param>
		/// <param name="inputs">List of input files/folders to check</param>
		/// <param name="outdir">Output directory to use to build to</param>
		/// <param name="tempdir">Temporary directory for archive extraction</param>
		/// <param name="externalScan">True to enable external scanning of archives, false otherwise</param>
		/// <param name="sevenzip">Integer representing the archive handling level for 7z</param>
		/// <param name="gz">Integer representing the archive handling level for GZip</param>
		/// <param name="rar">Integer representing the archive handling level for RAR</param>
		/// <param name="zip">Integer representing the archive handling level for Zip</param>
		/// <param name="logger">Logger object for file and console output</param>
		public SimpleSort(DatData datdata, List<string> inputs, string outdir, string tempdir,
			bool externalScan, int sevenzip, int gz, int rar, int zip, Logger logger)
		{
			_datdata = datdata;
			_inputs = inputs;
			_outdir = (outdir == "" ? "Rebuild" : outdir);
			_tempdir = (tempdir == "" ? "__TEMP__" : tempdir);
			_externalScan = externalScan;
			_7z = (ArchiveScanLevel)(sevenzip < 0 || sevenzip > 2 ? 0 : sevenzip);
			_gz = (ArchiveScanLevel)(gz < 0 || gz > 2 ? 0 : gz);
			_rar = (ArchiveScanLevel)(rar < 0 || rar > 2 ? 0 : rar);
			_zip = (ArchiveScanLevel)(zip < 0 || zip > 0 ? 0 : zip);
			_logger = logger;
		}

		/// <summary>
		/// Main entry point for the program
		/// </summary>
		/// <param name="args">List of arguments to be parsed</param>
		public static void Main(string[] args)
		{
			// If output is being redirected, don't allow clear screens
			if (!Console.IsOutputRedirected)
			{
				Console.Clear();
			}

			// Perform initial setup and verification
			Logger logger = new Logger(true, "simplesort.log");
			logger.Start();
			Remapping.CreateHeaderSkips();

			// Credits take precidence over all
			if ((new List<string>(args)).Contains("--credits"))
			{
				Build.Credits();
				return;
			}

			// If there's no arguments, show help
			if (args.Length == 0)
			{
				Build.Help();
				logger.Close();
				return;
			}

			// Output the title
			Build.Start("SimpleSort");

			// Set all default values
			bool help = false,
				externalScan = false,
				simpleSort = true;
			int sevenzip = 0,
				gz = 2,
				rar = 2,
				zip = 0;
			string outdir = "",
				tempdir = "";
			List<string> inputs = new List<string>();
			List<string> datfiles = new List<string>();

			// Determine which switches are enabled (with values if necessary)
			foreach (string arg in args)
			{
				switch (arg)
				{
					case "-?":
					case "-h":
					case "--help":
						help = true;
						break;
					case "-qs":
					case "--quick":
						externalScan = true;
						break;
					default:
						string temparg = arg.Replace("\"", "").Replace("file://", "");

						if (temparg.StartsWith("-7z=") || temparg.StartsWith("--7z="))
						{
							if (!Int32.TryParse(temparg.Split('=')[1], out sevenzip))
							{
								sevenzip = 0;
							}
						}
						else if (temparg.StartsWith("-dat=") || temparg.StartsWith("--dat="))
						{
							string datfile = temparg.Split('=')[1];
							if (!File.Exists(datfile))
							{
								logger.Error("DAT must be a valid file: " + datfile);
								Console.WriteLine();
								Build.Help();
								logger.Close();
								return;
							}
							datfiles.Add(datfile);
						}
						else if (temparg.StartsWith("-gz=") || temparg.StartsWith("--gz="))
						{
							if (!Int32.TryParse(temparg.Split('=')[1], out gz))
							{
								gz = 2;
							}
						}
						else if (temparg.StartsWith("-out=") || temparg.StartsWith("--out="))
						{
							outdir = temparg.Split('=')[1];
						}
						else if (temparg.StartsWith("-rar=") || temparg.StartsWith("--rar="))
						{
							if (!Int32.TryParse(temparg.Split('=')[1], out rar))
							{
								rar = 2;
							}
						}
						else if (temparg.StartsWith("-t=") || temparg.StartsWith("--temp="))
						{
							tempdir = temparg.Split('=')[1];
						}
						else if (temparg.StartsWith("-zip=") || temparg.StartsWith("--zip="))
						{
							if (!Int32.TryParse(temparg.Split('=')[1], out zip))
							{
								zip = 0;
							}
						}
						else if (File.Exists(temparg) || Directory.Exists(temparg))
						{
							inputs.Add(temparg);
						}
						else
						{
							logger.Error("Invalid input detected: " + arg);
							Console.WriteLine();
							Build.Help();
							logger.Close();
							return;
						}
						break;
				}
			}

			// If help is set, show the help screen
			if (help)
			{
				Build.Help();
				logger.Close();
				return;
			}

			// If a switch that requires a filename is set and no file is, show the help screen
			if (inputs.Count == 0 && (simpleSort))
			{
				logger.Error("This feature requires at least one input");
				Build.Help();
				logger.Close();
				return;
			}

			// If we are doing a simple sort
			if (simpleSort)
			{
				if (datfiles.Count > 0)
				{
					InitSimpleSort(datfiles, inputs, outdir, tempdir, externalScan, sevenzip, gz, rar, zip, logger);
				}
				else
				{
					logger.Error("A datfile is required to use this feature");
					Build.Help();
					logger.Close();
					return;
				}
			}

			// If nothing is set, show the help
			else
			{
				Build.Help();
			}

			logger.Close();
			return;
		}

		/// <summary>
		/// Wrap sorting files using an input DAT
		/// </summary>
		/// <param name="datfiles">Names of the DATs to compare against</param>
		/// <param name="inputs">List of input files/folders to check</param>
		/// <param name="outdir">Output directory to use to build to</param>
		/// <param name="tempdir">Temporary directory for archive extraction</param>
		/// <param name="externalScan">True to enable external scanning of archives, false otherwise</param>
		/// <param name="sevenzip">Integer representing the archive handling level for 7z</param>
		/// <param name="gz">Integer representing the archive handling level for GZip</param>
		/// <param name="rar">Integer representing the archive handling level for RAR</param>
		/// <param name="zip">Integer representing the archive handling level for Zip</param>
		/// <param name="logger">Logger object for file and console output</param>
		private static void InitSimpleSort(List<string> datfiles, List<string> inputs, string outdir, string tempdir, 
			bool externalScan, int sevenzip, int gz, int rar, int zip, Logger logger)
		{
			// Add all of the input DATs into one huge internal DAT
			DatData datdata = new DatData();
			foreach (string datfile in datfiles)
			{
				datdata = DatTools.Parse(datfile, 0, 0, datdata, logger);
			}

			SimpleSort ss = new SimpleSort(datdata, inputs, outdir, tempdir, externalScan, sevenzip, gz, rar, zip, logger);
			ss.RebuildFromFolder();
		}

		/// <summary>
		/// Process the DAT and find all matches in input files and folders
		/// </summary>
		/// <returns></returns>
		public bool RebuildFromFolder()
		{
			bool success = true;

			// First, check that the output directory exists
			if (!Directory.Exists(_outdir))
			{
				Directory.CreateDirectory(_outdir);
				_outdir = Path.GetFullPath(_outdir);
			}

			// Then create or clean the temp directory
			if (!Directory.Exists(_tempdir))
			{
				Directory.CreateDirectory(_tempdir);
			}
			else
			{
				Output.CleanDirectory(_tempdir);
			}

			// Then, loop through and check each of the inputs
			_logger.User("Starting to loop through inputs");
			foreach (string input in _inputs)
			{
				if (File.Exists(input))
				{
					_logger.Log("File found: '" + input + "'");
					success &= ProcessFile(input);
					Output.CleanDirectory(_tempdir);
				}
				else if (Directory.Exists(input))
				{
					_logger.Log("Directory found: '" + input + "'");
					foreach (string file in Directory.EnumerateFiles(input, "*", SearchOption.AllDirectories))
					{
						_logger.Log("File found: '" + file + "'");
						success &= ProcessFile(file);
						Output.CleanDirectory(_tempdir);
					}
				}
				else
				{
					_logger.Error("'" + input + "' is not a file or directory!");
				}
			}

			// Now one final delete of the temp directory
			while (Directory.Exists(_tempdir))
			{
				try
				{
					Directory.Delete(_tempdir, true);
				}
				catch
				{
					continue;
				}
			}

			return success;
		}

		/// <summary>
		/// Process an individual file against the DAT
		/// </summary>
		/// <param name="input">The name of the input file</param>
		/// <param name="recurse">True if this is in a recurse step and the file should be deleted, false otherwise (default)</param>
		/// <returns>True if it was processed properly, false otherwise</returns>
		private bool ProcessFile(string input, bool recurse = false)
		{
			bool success = true;

			// Get the full path of the input for movement purposes
			input = Path.GetFullPath(input);
			_logger.User("Beginning processing of '" + input + "'");

			// If we have an archive, scan it if necessary
			bool shouldscan = true;
			try
			{
				IArchive temp = ArchiveFactory.Open(input);
				switch (temp.Type)
				{
					case ArchiveType.GZip:
						shouldscan = (_gz != ArchiveScanLevel.Internal);
						break;
					case ArchiveType.Rar:
						shouldscan = (_rar != ArchiveScanLevel.Internal);
						break;
					case ArchiveType.SevenZip:
						shouldscan = (_7z != ArchiveScanLevel.Internal);
						break;
					case ArchiveType.Zip:
						shouldscan = (_zip != ArchiveScanLevel.Internal);
						break;
				}
			}
			catch
			{
				shouldscan = true;
			}

			// Hash and match the external files
			if (shouldscan)
			{
				RomData rom = RomTools.GetSingleFileInfo(input);

				// If we have a blank RomData, it's an error
				if (rom.Name == null)
				{
					return false;
				}

				// Try to find the matches to the file that was found
				List<RomData> foundroms = RomTools.GetDuplicates(rom, _datdata);
				_logger.User("File '" + input + "' had " + foundroms.Count + " matches in the DAT!");
				foreach (RomData found in foundroms)
				{
					_logger.Log("Matched name: " + found.Name);
					ArchiveTools.WriteToArchive(input, _outdir, found);
				}

				// Now get the headerless file if it exists
				int hs = 0;
				RomTools.GetFileHeaderType(input, out hs, _logger);
				if (hs > 0)
				{
					string newinput = input + ".new";
					_logger.Log("Creating unheadered file: '" + newinput + "'");
					Output.RemoveBytesFromFile(input, newinput, hs, 0);
					RomData drom = RomTools.GetSingleFileInfo(newinput);

					// If we have a blank RomData, it's an error
					if (drom.Name == null)
					{
						return false;
					}

					// Try to find the matches to the file that was found
					List<RomData> founddroms = RomTools.GetDuplicates(drom, _datdata);
					_logger.User("File '" + newinput + "' had " + founddroms.Count + " matches in the DAT!");
					foreach (RomData found in founddroms)
					{
						// First output the headerless rom
						_logger.Log("Matched name: " + found.Name);
						ArchiveTools.WriteToArchive(newinput, _outdir, found);

						// Then output the headered rom (renamed)
						RomData newfound = found;
						newfound.Name = Path.GetFileNameWithoutExtension(newfound.Name) + " (" + rom.CRC + ")" + Path.GetExtension(newfound.Name);

						_logger.Log("Matched name: " + newfound.Name);
						ArchiveTools.WriteToArchive(input, _outdir, newfound);
					}

					// Now remove this temporary file
					try
					{
						File.Delete(newinput);
					}
					catch
					{
						// Don't log file deletion errors
					}
				}
			}

			// If external scanning is enabled, use that method instead
			if (_externalScan)
			{
				_logger.Log("Beginning quick scan of contents from '" + input + "'");
				List<RomData> internalRomData = ArchiveTools.GetArchiveFileInfo(input, _logger);
				_logger.Log(internalRomData.Count + " entries found in '" + input + "'");

				// If the list is populated, then the file was a filled archive
				if (internalRomData.Count > 0)
				{
					foreach (RomData rom in internalRomData)
					{
						// Try to find the matches to the file that was found
						List<RomData> foundroms = RomTools.GetDuplicates(rom, _datdata);
						_logger.User("File '" + rom.Name + "' had " + foundroms.Count + " matches in the DAT!");
						foreach (RomData found in foundroms)
						{
							_logger.Log("Matched name: " + found.Name);
							string newinput = ArchiveTools.ExtractSingleItemFromArchive(input, rom.Name, _tempdir, _logger);
							if (newinput != null && File.Exists(newinput))
							{
								ArchiveTools.WriteToArchive(newinput, _outdir, found);
								try
								{
									File.Delete(newinput);
								}
								catch (Exception ex)
								{
									// Don't log file deletion errors
								}
							}
						}
					}
				}
			}
			else
			{
				// If the file isn't an archive, skip out sooner
				if (ArchiveTools.GetCurrentArchiveType(input, _logger) == null)
				{
					// Remove the current file if we are in recursion so it's not picked up in the next step
					if (recurse)
					{
						try
						{
							File.Delete(input);
						}
						catch (Exception ex)
						{
							// Don't log file deletion errors
						}
					}

					return success;
				}

				// Now, if the file is a supported archive type, also run on all files within
				bool encounteredErrors = !ArchiveTools.ExtractArchive(input, _tempdir, _7z, _gz, _rar, _zip, _logger);

				// Remove the current file if we are in recursion so it's not picked up in the next step
				if (recurse)
				{
					try
					{
						File.Delete(input);
					}
					catch (Exception ex)
					{
						// Don't log file deletion errors
					}
				}

				// If no errors were encountered, we loop through the temp directory
				if (!encounteredErrors)
				{
					_logger.User("Archive found! Successfully extracted");
					foreach (string file in Directory.EnumerateFiles(_tempdir, "*", SearchOption.AllDirectories))
					{
						success &= ProcessFile(file, true);
					}
				}
			}

			return success;
		}
	}
}