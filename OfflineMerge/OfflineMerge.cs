﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SabreTools.Helper;

namespace SabreTools
{
	public class OfflineMerge
	{
		// Instance variables
		private string _currentAllMerged;
		private string _currentMissingMerged;
		private string _currentNewMerged;
		private bool _fake;
		private Logger _logger;

		// Static required variables
		private static long sizezero = 0;
		private static string crczero = "00000000";
		private static string md5zero = "d41d8cd98f00b204e9800998ecf8427e";
		private static string sha1zero = "da39a3ee5e6b4b0d3255bfef95601890afd80709";

		/// <summary>
		/// Instantiate an OfflineMerge object
		/// </summary>
		/// <param name="currentAllMerged">Old-current DAT with merged and deduped values</param>
		/// <param name="currentMissingMerged">Old-current missing DAT with merged and deduped values</param>
		/// <param name="currentNewMerged">New-current DAT with merged and deduped values</param>
		/// <param name="fake">True if all values should be replaced with default 0-byte values, false otherwise</param>
		/// <param name="logger">Logger object for console and file output</param>
		public OfflineMerge (string currentAllMerged, string currentMissingMerged, string currentNewMerged, bool fake, Logger logger)
		{
			_currentAllMerged = currentAllMerged.Replace("\"", "");
			_currentMissingMerged = currentMissingMerged.Replace("\"", "");
			_currentNewMerged = currentNewMerged.Replace("\"", "");
			_fake = fake;
			_logger = logger;
		}

		public static void Main(string[] args)
		{
			// Perform initial setup and verification
			Logger logger = new Logger(false, "database.log");
			logger.Start();
			Console.Clear();

			// Credits take precidence over all
			if ((new List<string>(args)).Contains("--credits"))
			{
				Build.Credits();
				logger.Close();
				return;
			}

			// If there's no arguments, show the help
			if (args.Length == 0)
			{
				Build.Help();
				logger.Close();
				return;
			}

			// Set all default values
			bool help = false, fake = false;
			string currentAllMerged = "", currentMissingMerged = "", currentNewMerged = "";

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
					case "-f":
					case "--fake":
						fake = true;
						break;
					default:
						string temparg = arg.Replace("\"", "");
						if (temparg.StartsWith("com="))
						{
							currentAllMerged = temparg.Split('=')[1];
						}
						else if (temparg.StartsWith("fix="))
						{
							currentMissingMerged = temparg.Split('=')[1];
						}
						else if (temparg.StartsWith("new="))
						{
							currentNewMerged = temparg.Split('=')[1];
						}
						else
						{
							logger.Warning("Invalid input detected: " + arg);
							Console.WriteLine();
							Build.Help();
							logger.Close();
							return;
						}
						break;
				}
			}

			// If help is set or all of the inputs are empty, show help
			if (help || (currentAllMerged == "" && currentMissingMerged == "" && currentNewMerged == ""))
			{
				Build.Help();
				logger.Close();
				return;
			}

			// Otherwise, run the program
			OfflineMerge om = new OfflineMerge(currentAllMerged, currentMissingMerged, currentNewMerged, fake, logger);
			bool success = om.Process();
			if (!success)
			{
				logger.Warning("At least one complete DAT and the fixdat is needed to run!");
			}
		}

		/// <summary>
		/// Process the supplied inputs and create the four outputs
		/// </summary>
		/// <returns>True if the files were created properly, false otherwise</returns>
		public bool Process()
		{
			// Check all of the files for validity and break if one doesn't exist
			if (_currentAllMerged != "" && !File.Exists(_currentAllMerged))
			{
				return false;
			}
			if (_currentMissingMerged != "" && !File.Exists(_currentMissingMerged))
			{
				return false;
			}
			if (_currentNewMerged != "" && !File.Exists(_currentNewMerged))
			{
				return false;
			}

			Console.WriteLine(_currentAllMerged + " " + _currentMissingMerged + " " + _currentNewMerged);

			// If we have all three DATs, then generate everything
			if (_currentAllMerged != "" && _currentMissingMerged != "" && _currentNewMerged != "")
			{
				// First get the combination Dictionary of currentAllMerged and currentNewMerged
				_logger.Log("Adding Current and New Merged DATs to the dictionary");
				Dictionary<string, List<RomData>> completeDats = new Dictionary<string, List<RomData>>();
				completeDats = RomManipulation.ParseDict(_currentAllMerged, 0, 0, completeDats, _logger);
				completeDats = RomManipulation.ParseDict(_currentNewMerged, 0, 0, completeDats, _logger);

				// Now get Net New output dictionary [(currentNewMerged)-(currentAllMerged)]
				_logger.Log("Creating and populating Net New dictionary");
				Dictionary<string, List<RomData>> netNew = new Dictionary<string, List<RomData>>();
				foreach (string key in completeDats.Keys)
				{
					List<RomData> templist = RomManipulation.Merge(completeDats[key]);
					foreach (RomData rom in templist)
					{
						if (!rom.Dupe && rom.System == _currentNewMerged)
						{
							if (netNew.ContainsKey(key))
							{
								netNew[key].Add(rom);
							}
							else
							{
								List<RomData> temp = new List<RomData>();
								temp.Add(rom);
								netNew.Add(key, temp);
							}
						}
					}
				}

				// Now create the Unneeded dictionary [(currentAllMerged)-(currentNewMerged)]
				_logger.Log("Creating and populating Uneeded dictionary");
				Dictionary<string, List<RomData>> unneeded = new Dictionary<string, List<RomData>>();
				foreach (string key in completeDats.Keys)
				{
					List<RomData> templist = RomManipulation.Merge(completeDats[key]);
					foreach (RomData rom in templist)
					{
						if (!rom.Dupe && rom.System == _currentAllMerged)
						{
							if (unneeded.ContainsKey(key))
							{
								unneeded[key].Add(rom);
							}
							else
							{
								List<RomData> temp = new List<RomData>();
								temp.Add(rom);
								unneeded.Add(key, temp);
							}
						}
					}
				}

				// Now create the New Missing dictionary [(Net New)+(currentMissingMerged-(Unneeded))]
				_logger.Log("Creating and populating New Missing dictionary");
				Dictionary<string, List<RomData>> midMissing = new Dictionary<string, List<RomData>>();
				midMissing = RomManipulation.ParseDict(_currentMissingMerged, 0, 0, midMissing, _logger);
				foreach (string key in unneeded.Keys)
				{
					if (midMissing.ContainsKey(key))
					{
						midMissing[key].AddRange(unneeded[key]);
					}
					else
					{
						midMissing.Add(key, unneeded[key]);
					}
				}
				Dictionary<string, List<RomData>> newMissing = new Dictionary<string, List<RomData>>();
				foreach (string key in midMissing.Keys)
				{
					List<RomData> templist = RomManipulation.Merge(midMissing[key]);
					foreach (RomData rom in templist)
					{
						if (!rom.Dupe && rom.System == _currentMissingMerged)
						{
							if (newMissing.ContainsKey(key))
							{
								newMissing[key].Add(rom);
							}
							else
							{
								List<RomData> temp = new List<RomData>();
								temp.Add(rom);
								newMissing.Add(key, temp);
							}
						}
					}
				}
				foreach (string key in netNew.Keys)
				{
					if (newMissing.ContainsKey(key))
					{
						newMissing[key].AddRange(netNew[key]);
					}
					else
					{
						newMissing.Add(key, netNew[key]);
					}
				}

				// Now create the Have dictionary [(currentNewMerged)-(c)]
				_logger.Log("Creating and populating Have dictionary");
				Dictionary<string, List<RomData>> midHave = new Dictionary<string, List<RomData>>();
				foreach (string key in newMissing.Keys)
				{
					if (midHave.ContainsKey(key))
					{
						midHave[key].AddRange(newMissing[key]);
					}
					else
					{
						midHave.Add(key, newMissing[key]);
					}
				}
				foreach (string key in completeDats.Keys)
				{
					if (midHave.ContainsKey(key))
					{
						foreach (RomData rom in completeDats[key])
						{
							if (rom.System == _currentNewMerged)
							{
								midHave[key].Add(rom);
							}
						}
					}
					else
					{
						List<RomData> roms = new List<RomData>();
						foreach (RomData rom in completeDats[key])
						{
							if (rom.System == _currentNewMerged)
							{
								roms.Add(rom);
							}
						}
						midHave.Add(key, roms);
					}
				}
				Dictionary<string, List<RomData>> have = new Dictionary<string, List<RomData>>();
				foreach (string key in midHave.Keys)
				{
					List<RomData> templist = RomManipulation.Merge(midHave[key]);
					foreach (RomData rom in templist)
					{
						if (!rom.Dupe && rom.System == _currentNewMerged)
						{
							if (have.ContainsKey(key))
							{
								have[key].Add(rom);
							}
							else
							{
								List<RomData> temp = new List<RomData>();
								temp.Add(rom);
								have.Add(key, temp);
							}
						}
					}
				}

				// If we are supposed to replace everything in the output with default values, do so
				if (_fake)
				{
					_logger.Log("Replacing all hashes in Net New with 0-byte values");
					List<string> keys = netNew.Keys.ToList();
					foreach (string key in keys)
					{
						List<RomData> temp = new List<RomData>();
						List<RomData> roms = netNew[key];
						for (int i = 0; i < roms.Count; i++)
						{
							RomData rom = roms[i];
							rom.Size = sizezero;
							rom.CRC = crczero;
							rom.MD5 = md5zero;
							rom.SHA1 = sha1zero;
							temp.Add(rom);
						}
						netNew[key] = temp;
					}

					_logger.Log("Replacing all hashes in Unneeded with 0-byte values");
					keys = unneeded.Keys.ToList();
					foreach (string key in keys)
					{
						List<RomData> temp = new List<RomData>();
						List<RomData> roms = unneeded[key];
						for (int i = 0; i < roms.Count; i++)
						{
							RomData rom = roms[i];
							rom.Size = sizezero;
							rom.CRC = crczero;
							rom.MD5 = md5zero;
							rom.SHA1 = sha1zero;
							temp.Add(rom);
						}
						unneeded[key] = temp;
					}

					_logger.Log("Replacing all hashes in New Missing with 0-byte values");
					keys = newMissing.Keys.ToList();
					foreach (string key in keys)
					{
						List<RomData> temp = new List<RomData>();
						List<RomData> roms = newMissing[key];
						for (int i = 0; i < roms.Count; i++)
						{
							RomData rom = roms[i];
							rom.Size = sizezero;
							rom.CRC = crczero;
							rom.MD5 = md5zero;
							rom.SHA1 = sha1zero;
							temp.Add(rom);
						}
						newMissing[key] = temp;
					}

					_logger.Log("Replacing all hashes in Have with 0-byte values");
					keys = have.Keys.ToList();
					foreach (string key in keys)
					{
						List<RomData> temp = new List<RomData>();
						List<RomData> roms = have[key];
						for (int i = 0; i < roms.Count; i++)
						{
							RomData rom = roms[i];
							rom.Size = sizezero;
							rom.CRC = crczero;
							rom.MD5 = md5zero;
							rom.SHA1 = sha1zero;
							temp.Add(rom);
						}
						have[key] = temp;
					}
				}

				// Finally, output all of the files
				Output.WriteToDatFromDict("Net New", "Net New", "", DateTime.Now.ToString("yyyy-MM-dd"), "", "SabreTools", false, false, true, "", netNew, _logger);
				Output.WriteToDatFromDict("Unneeded", "Unneeded", "", DateTime.Now.ToString("yyyy-MM-dd"), "", "SabreTools", false, false, true, "", unneeded, _logger);
				Output.WriteToDatFromDict("New Missing", "New Missing", "", DateTime.Now.ToString("yyyy-MM-dd"), "", "SabreTools", false, false, true, "", newMissing, _logger);
				Output.WriteToDatFromDict("Have", "Have", "", DateTime.Now.ToString("yyyy-MM-dd"), "", "SabreTools", false, false, true, "", have, _logger);

				return true;
			}

			// If we only have the old merged and missing, only generate Have
			else if (_currentAllMerged != "" && _currentMissingMerged != "")
			{
				// Now create the Have dictionary [(currentAllMerged)-(currentMissingMerged)]
				_logger.Log("Creating and populating Have dictionary");
				Dictionary<string, List<RomData>> midHave = new Dictionary<string, List<RomData>>();
				midHave = RomManipulation.ParseDict(_currentMissingMerged, 0, 0, midHave, _logger);
				midHave = RomManipulation.ParseDict(_currentAllMerged, 0, 0, midHave, _logger);
				Dictionary<string, List<RomData>> have = new Dictionary<string, List<RomData>>();
				foreach (string key in midHave.Keys)
				{
					List<RomData> templist = RomManipulation.Merge(midHave[key]);
					foreach (RomData rom in templist)
					{
						if (!rom.Dupe && rom.System == _currentAllMerged)
						{
							if (have.ContainsKey(key))
							{
								have[key].Add(rom);
							}
							else
							{
								List<RomData> temp = new List<RomData>();
								temp.Add(rom);
								have.Add(key, temp);
							}
						}
					}
				}

				// If we are supposed to replace everything in the output with default values, do so
				if (_fake)
				{
					_logger.Log("Replacing all hashes in Have with 0-byte values");
					List<string> keys = have.Keys.ToList();
					foreach (string key in keys)
					{
						List<RomData> temp = new List<RomData>();
						List<RomData> roms = have[key];
						for (int i = 0; i < roms.Count; i++)
						{
							RomData rom = roms[i];
							rom.Size = sizezero;
							rom.CRC = crczero;
							rom.MD5 = md5zero;
							rom.SHA1 = sha1zero;
							temp.Add(rom);
						}
						have[key] = temp;
					}
				}

				Output.WriteToDatFromDict("Have", "Have", "", DateTime.Now.ToString("yyyy-MM-dd"), "", "SabreTools", false, false, true, "", have, _logger);

				return true;
			}

			// If we only have the new merged and missing, only generate Have
			else if (_currentNewMerged != "" && _currentMissingMerged != "")
			{
				// Now create the Have dictionary [(currentNewMerged)-(currentMissingMerged)]
				_logger.Log("Creating and populating Have dictionary");
				Dictionary<string, List<RomData>> midHave = new Dictionary<string, List<RomData>>();
				midHave = RomManipulation.ParseDict(_currentMissingMerged, 0, 0, midHave, _logger);
				midHave = RomManipulation.ParseDict(_currentNewMerged, 0, 0, midHave, _logger);
				Dictionary<string, List<RomData>> have = new Dictionary<string, List<RomData>>();
				foreach (string key in midHave.Keys)
				{
					List<RomData> templist = RomManipulation.Merge(midHave[key]);
					foreach (RomData rom in templist)
					{
						if (!rom.Dupe && rom.System == _currentNewMerged)
						{
							if (have.ContainsKey(key))
							{
								have[key].Add(rom);
							}
							else
							{
								List<RomData> temp = new List<RomData>();
								temp.Add(rom);
								have.Add(key, temp);
							}
						}
					}
				}

				// If we are supposed to replace everything in the output with default values, do so
				if (_fake)
				{
					_logger.Log("Replacing all hashes in Have with 0-byte values");
					List<string> keys = have.Keys.ToList();
					foreach (string key in keys)
					{
						List<RomData> temp = new List<RomData>();
						List<RomData> roms = have[key];
						for (int i = 0; i < roms.Count; i++)
						{
							RomData rom = roms[i];
							rom.Size = sizezero;
							rom.CRC = crczero;
							rom.MD5 = md5zero;
							rom.SHA1 = sha1zero;
							temp.Add(rom);
						}
						have[key] = temp;
					}
				}

				Output.WriteToDatFromDict("Have", "Have", "", DateTime.Now.ToString("yyyy-MM-dd"), "", "SabreTools", false, false, true, "", have, _logger);

				return true;
			}

			return false;
		}
	}
}
