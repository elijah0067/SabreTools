﻿using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.Helper.Data;
using SabreTools.Helper.Tools;
using NaturalSort;

namespace SabreTools.Helper.Dats
{
	[Serializable]
	public abstract class DatItem : IEquatable<DatItem>, IComparable<DatItem>
	{
		#region Protected instance variables

		// Standard item information
		protected string _name;
		protected ItemType _itemType;
		protected DupeType _dupeType;

		// Machine information
		protected Machine _machine;

		// Software list information
		protected bool? _supported;
		protected string _publisher;
		protected List<Tuple<string, string>> _infos;
		protected string _partName;
		protected string _partInterface;
		protected List<Tuple<string, string>> _features;
		protected string _areaName;
		protected long? _areaSize;

		// Source metadata information
		protected int _systemId;
		protected string _systemName;
		protected int _sourceId;
		protected string _sourceName;

		#endregion

		#region Publicly facing variables

		// Standard item information
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		public ItemType Type
		{
			get { return _itemType; }
			set { _itemType = value; }
		}
		public DupeType Dupe
		{
			get { return _dupeType; }
			set { _dupeType = value; }
		}

		// Machine information
		public Machine Machine
		{
			get { return _machine; }
			set { _machine = value; }
		}

		// Software list information
		public bool? Supported
		{
			get { return _supported; }
			set { _supported = value; }
		}
		public string Publisher
		{
			get { return _publisher; }
			set { _publisher = value; }
		}
		public List<Tuple<string, string>> Infos
		{
			get { return _infos; }
			set { _infos = value; }
		}
		public string PartName
		{
			get { return _partName; }
			set { _partName = value; }
		}
		public string PartInterface
		{
			get { return _partInterface; }
			set { _partInterface = value; }
		}
		public List<Tuple<string, string>> Features
		{
			get { return _features; }
			set { _features = value; }
		}
		public string AreaName
		{
			get { return _areaName; }
			set { _areaName = value; }
		}
		public long? AreaSize
		{
			get { return _areaSize; }
			set { _areaSize = value; }
		}

		// Source metadata information
		public int SystemID
		{
			get { return _systemId; }
			set { _systemId = value; }
		}
		public string System
		{
			get { return _systemName; }
			set { _systemName = value; }
		}
		public int SourceID
		{
			get { return _sourceId; }
			set { _sourceId = value; }
		}
		public string Source
		{
			get { return _sourceName; }
			set { _sourceName = value; }
		}

		#endregion

		#region Comparision Methods

		public int CompareTo(DatItem other)
		{
			int ret = 0;

			try
			{
				if (_name == other.Name)
				{
					ret = (this.Equals(other) ? 0 : 1);
				}
				ret = String.Compare(_name, other.Name);
			}
			catch
			{
				ret = 1;
			}

			return ret;
		}

		public abstract bool Equals(DatItem other);

		/// <summary>
		/// Determine if an item is a duplicate using partial matching logic
		/// </summary>
		/// <param name="lastItem">DatItem to use as a baseline</param>
		/// <param name="logger">Logger object for console and/or file output</param>
		/// <returns>True if the roms are duplicates, false otherwise</returns>
		public bool IsDuplicate(DatItem lastItem, Logger logger)
		{
			bool dupefound = this.Equals(lastItem);

			// More wonderful SHA-1 logging that has to be done
			if (_itemType == ItemType.Rom)
			{
				if (((Rom)this).SHA1 == ((Rom)lastItem).SHA1 && ((Rom)this).Size != ((Rom)lastItem).Size)
				{
					logger.User("SHA-1 mismatch - Hash: " + ((Rom)this).SHA1);
				}
			}

			return dupefound;
		}

		/// <summary>
		/// Return the duplicate status of two items
		/// </summary>
		/// <param name="lastItem">DatItem to check against</param>
		/// <param name="logger">Logger object for console and/or file output</param>
		/// <returns>The DupeType corresponding to the relationship between the two</returns>
		public DupeType GetDuplicateStatus(DatItem lastItem, Logger logger)
		{
			DupeType output = 0x00;

			// If we don't have a duplicate at all, return none
			if (!this.IsDuplicate(lastItem, logger))
			{
				return output;
			}

			// If the duplicate is external already or should be, set it
			if ((lastItem.Dupe & DupeType.External) != 0 || lastItem.SystemID != this.SystemID || lastItem.SourceID != this.SourceID)
			{
				if (lastItem.Machine.Name == this.Machine.Name && lastItem.Name == this.Name)
				{
					output = DupeType.External | DupeType.All;
				}
				else
				{
					output = DupeType.External | DupeType.Hash;
				}
			}

			// Otherwise, it's considered an internal dupe
			else
			{
				if (lastItem.Machine.Name == this.Machine.Name && lastItem.Name == this.Name)
				{
					output = DupeType.Internal | DupeType.All;
				}
				else
				{
					output = DupeType.Internal | DupeType.Hash;
				}
			}

			return output;
		}

		#endregion

		#region Instance Methods

		#region Sorting and Merging

		/// <summary>
		/// Determine if a rom should be included based on filters
		/// </summary>
		/// <param name="gamename">Name of the game to match (can use asterisk-partials)</param>
		/// <param name="romname">Name of the rom to match (can use asterisk-partials)</param>
		/// <param name="romtype">Type of the rom to match</param>
		/// <param name="sgt">Find roms greater than or equal to this size</param>
		/// <param name="slt">Find roms less than or equal to this size</param>
		/// <param name="seq">Find roms equal to this size</param>
		/// <param name="crc">CRC of the rom to match (can use asterisk-partials)</param>
		/// <param name="md5">MD5 of the rom to match (can use asterisk-partials)</param>
		/// <param name="sha1">SHA-1 of the rom to match (can use asterisk-partials)</param>
		/// <param name="itemStatus">Select roms with the given status</param>
		/// <param name="logger">Logging object for console and file output</param>
		/// <returns>Returns true if it should be included, false otherwise</returns>
		public bool Filter(string gamename, string romname, string romtype, long sgt,
			long slt, long seq, string crc, string md5, string sha1, ItemStatus itemStatus, Logger logger)
		{
			// Take care of Rom and Disk specific differences
			if (Type == ItemType.Rom)
			{
				Rom rom = (Rom)this;

				// Filter on status
				if (itemStatus != ItemStatus.NULL)
				{
					if (itemStatus == ItemStatus.NotNodump && rom.ItemStatus == ItemStatus.Nodump)
					{
						return false;
					}
					else if (itemStatus != ItemStatus.NotNodump && rom.ItemStatus != itemStatus)
					{
						return false;
					}
				}

				// Filter on rom size
				if (seq != -1 && rom.Size != seq)
				{
					return false;
				}
				else
				{
					if (sgt != -1 && rom.Size < sgt)
					{
						return false;
					}
					if (slt != -1 && rom.Size > slt)
					{
						return false;
					}
				}

				// Filter on crc
				if (!String.IsNullOrEmpty(crc))
				{
					if (crc.StartsWith("*") && crc.EndsWith("*"))
					{
						if (!rom.CRC.ToLowerInvariant().Contains(crc.ToLowerInvariant().Replace("*", "")))
						{
							return false;
						}
					}
					else if (crc.StartsWith("*"))
					{
						if (!rom.CRC.EndsWith(crc.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
					else if (crc.EndsWith("*"))
					{
						if (!rom.CRC.StartsWith(crc.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
					else
					{
						if (!String.Equals(rom.CRC, crc, StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
				}

				// Filter on md5
				if (!String.IsNullOrEmpty(md5))
				{
					if (md5.StartsWith("*") && md5.EndsWith("*"))
					{
						if (!rom.MD5.ToLowerInvariant().Contains(md5.ToLowerInvariant().Replace("*", "")))
						{
							return false;
						}
					}
					else if (md5.StartsWith("*"))
					{
						if (!rom.MD5.EndsWith(md5.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
					else if (md5.EndsWith("*"))
					{
						if (!rom.MD5.StartsWith(md5.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
					else
					{
						if (!String.Equals(rom.MD5, md5, StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
				}

				// Filter on sha1
				if (!String.IsNullOrEmpty(sha1))
				{
					if (sha1.StartsWith("*") && sha1.EndsWith("*"))
					{
						if (!rom.SHA1.ToLowerInvariant().Contains(sha1.ToLowerInvariant().Replace("*", "")))
						{
							return false;
						}
					}
					else if (sha1.StartsWith("*"))
					{
						if (!rom.SHA1.EndsWith(sha1.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
					else if (sha1.EndsWith("*"))
					{
						if (!rom.SHA1.StartsWith(sha1.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
					else
					{
						if (!String.Equals(rom.SHA1, sha1, StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
				}
			}
			else if (Type == ItemType.Disk)
			{
				Disk rom = (Disk)this;

				// Filter on status
				if (itemStatus != ItemStatus.NULL && rom.ItemStatus != itemStatus)
				{
					if (itemStatus == ItemStatus.NotNodump && rom.ItemStatus == ItemStatus.Nodump)
					{
						return false;
					}
					else if (itemStatus != ItemStatus.NotNodump && rom.ItemStatus != itemStatus)
					{
						return false;
					}
				}

				// Filter on md5
				if (!String.IsNullOrEmpty(md5))
				{
					if (md5.StartsWith("*") && md5.EndsWith("*"))
					{
						if (!rom.MD5.ToLowerInvariant().Contains(md5.ToLowerInvariant().Replace("*", "")))
						{
							return false;
						}
					}
					else if (md5.StartsWith("*"))
					{
						if (!rom.MD5.EndsWith(md5.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
					else if (md5.EndsWith("*"))
					{
						if (!rom.MD5.StartsWith(md5.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
					else
					{
						if (!String.Equals(rom.MD5, md5, StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
				}

				// Filter on sha1
				if (!String.IsNullOrEmpty(sha1))
				{
					if (sha1.StartsWith("*") && sha1.EndsWith("*"))
					{
						if (!rom.SHA1.ToLowerInvariant().Contains(sha1.ToLowerInvariant().Replace("*", "")))
						{
							return false;
						}
					}
					else if (sha1.StartsWith("*"))
					{
						if (!rom.SHA1.EndsWith(sha1.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
					else if (sha1.EndsWith("*"))
					{
						if (!rom.SHA1.StartsWith(sha1.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
					else
					{
						if (!String.Equals(rom.SHA1, sha1, StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
				}
			}

			// Filter on game name
			if (!String.IsNullOrEmpty(gamename))
			{
				if (gamename.StartsWith("*") && gamename.EndsWith("*"))
				{
					if (!Machine.Name.ToLowerInvariant().Contains(gamename.ToLowerInvariant().Replace("*", "")))
					{
						return false;
					}
				}
				else if (gamename.StartsWith("*"))
				{
					if (!Machine.Name.EndsWith(gamename.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase))
					{
						return false;
					}
				}
				else if (gamename.EndsWith("*"))
				{
					if (!Machine.Name.StartsWith(gamename.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase))
					{
						return false;
					}
				}
				else
				{
					if (!String.Equals(Machine.Name, gamename, StringComparison.InvariantCultureIgnoreCase))
					{
						return false;
					}
				}
			}

			// Filter on rom name
			if (!String.IsNullOrEmpty(romname))
			{
				if (romname.StartsWith("*") && romname.EndsWith("*"))
				{
					if (!Name.ToLowerInvariant().Contains(romname.ToLowerInvariant().Replace("*", "")))
					{
						return false;
					}
				}
				else if (romname.StartsWith("*"))
				{
					if (!Name.EndsWith(romname.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase))
					{
						return false;
					}
				}
				else if (romname.EndsWith("*"))
				{
					if (!Name.StartsWith(romname.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase))
					{
						return false;
					}
				}
				else
				{
					if (!String.Equals(Name, romname, StringComparison.InvariantCultureIgnoreCase))
					{
						return false;
					}
				}
			}

			// Filter on rom type
			if (String.IsNullOrEmpty(romtype) && Type != ItemType.Rom && Type != ItemType.Disk)
			{
				return false;
			}
			if (!String.IsNullOrEmpty(romtype) && !String.Equals(Type.ToString(), romtype, StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Check if a DAT contains the given rom
		/// </summary>
		/// <param name="datdata">Dat to match against</param>
		/// <param name="logger">Logger object for console and/or file output</param>
		/// <returns>True if it contains the rom, false otherwise</returns>
		public bool HasDuplicates(DatFile datdata, Logger logger)
		{
			// Check for an empty rom list first
			if (datdata.Files == null || datdata.Files.Count == 0)
			{
				return false;
			}

			// Get the correct dictionary based on what is available
			string key = "";
			if (_itemType == ItemType.Rom && ((Rom)this).CRC != null)
			{
				key = ((Rom)this).CRC;
				datdata.BucketByCRC(false, logger, false);
			}
			else if (_itemType == ItemType.Rom && ((Rom)this).MD5 != null)
			{
				key = ((Rom)this).MD5;
				datdata.BucketByMD5(false, logger, false);
			}
			else if (_itemType == ItemType.Disk && ((Disk)this).MD5 != null)
			{
				key = ((Disk)this).MD5;
				datdata.BucketByMD5(false, logger, false);
			}
			else if (_itemType == ItemType.Rom && ((Rom)this).SHA1 != null)
			{
				key = ((Rom)this).SHA1;
				datdata.BucketBySHA1(false, logger, false);
			}
			else if (_itemType == ItemType.Disk && ((Disk)this).SHA1 != null)
			{
				key = ((Disk)this).SHA1;
				datdata.BucketBySHA1(false, logger, false);
			}
			else if (_itemType == ItemType.Rom)
			{
				key = ((Rom)this).Size.ToString();
				datdata.BucketBySize(false, logger, false);
			}
			else
			{
				key = "-1";
				datdata.BucketBySize(false, logger, false);
			}

			// If the key doesn't exist, return the empty list
			if (!datdata.Files.ContainsKey(key))
			{
				return false;
			}

			// Try to find duplicates
			List<DatItem> roms = datdata.Files[key];

			foreach (DatItem rom in roms)
			{
				if (IsDuplicate(rom, logger))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// List all duplicates found in a DAT based on a rom
		/// </summary>
		/// <param name="datdata">Dat to match against</param>
		/// <param name="logger">Logger object for console and/or file output</param>
		/// <param name="remove">True to remove matched roms from the input, false otherwise (default)</param>
		/// <returns>List of matched DatItem objects</returns>
		public List<DatItem> GetDuplicates(DatFile datdata, Logger logger, bool remove = false)
		{
			List<DatItem> output = new List<DatItem>();

			// Check for an empty rom list first
			if (datdata.Files == null || datdata.Files.Count == 0)
			{
				return output;
			}

			// Get the correct dictionary based on what is available
			string key = "";
			if (_itemType == ItemType.Rom && ((Rom)this).CRC != null)
			{
				key = ((Rom)this).CRC;
				datdata.BucketByCRC(false, logger, false);
			}
			else if (_itemType == ItemType.Rom && ((Rom)this).MD5 != null)
			{
				key = ((Rom)this).MD5;
				datdata.BucketByMD5(false, logger, false);
			}
			else if (_itemType == ItemType.Disk && ((Disk)this).MD5 != null)
			{
				key = ((Disk)this).MD5;
				datdata.BucketByMD5(false, logger, false);
			}
			else if (_itemType == ItemType.Rom && ((Rom)this).SHA1 != null)
			{
				key = ((Rom)this).SHA1;
				datdata.BucketBySHA1(false, logger, false);
			}
			else if (_itemType == ItemType.Disk && ((Disk)this).SHA1 != null)
			{
				key = ((Disk)this).SHA1;
				datdata.BucketBySHA1(false, logger, false);
			}
			else if (_itemType == ItemType.Rom)
			{
				key = ((Rom)this).Size.ToString();
				datdata.BucketBySize(false, logger, false);
			}
			else
			{
				key = "-1";
				datdata.BucketBySize(false, logger, false);
			}

			// If the key doesn't exist, return the empty list
			if (!datdata.Files.ContainsKey(key))
			{
				return output;
			}

			// Try to find duplicates
			List<DatItem> roms = datdata.Files[key];
			List<DatItem> left = new List<DatItem>();

			foreach (DatItem rom in roms)
			{
				if (IsDuplicate(rom, logger))
				{
					output.Add(rom);
				}
				else
				{
					left.Add(rom);
				}
			}

			// If we're in removal mode, replace the list with the new one
			if (remove)
			{
				datdata.Files[key] = left;
			}

			return output;
		}

		#endregion

		#endregion // Instance Methods

		#region Static Methods

		#region Sorting and Merging

		/// <summary>
		/// Merge an arbitrary set of ROMs based on the supplied information
		/// </summary>
		/// <param name="infiles">List of File objects representing the roms to be merged</param>
		/// <param name="logger">Logger object for console and/or file output</param>
		/// <returns>A List of RomData objects representing the merged roms</returns>
		public static List<DatItem> Merge(List<DatItem> infiles, Logger logger)
		{
			// Check for null or blank roms first
			if (infiles == null || infiles.Count == 0)
			{
				return new List<DatItem>();
			}

			// Create output list
			List<DatItem> outfiles = new List<DatItem>();

			// Then deduplicate them by checking to see if data matches previous saved roms
			foreach (DatItem file in infiles)
			{
				// If it's a nodump, add and skip
				if (file.Type == ItemType.Rom && ((Rom)file).ItemStatus == ItemStatus.Nodump)
				{
					outfiles.Add(file);
					continue;
				}
				else if (file.Type == ItemType.Disk && ((Disk)file).ItemStatus == ItemStatus.Nodump)
				{
					outfiles.Add(file);
					continue;
				}

				// If it's the first rom in the list, don't touch it
				if (outfiles.Count != 0)
				{
					// Check if the rom is a duplicate
					DupeType dupetype = 0x00;
					DatItem saveditem = new Rom();
					int pos = -1;
					for (int i = 0; i < outfiles.Count; i++)
					{
						DatItem lastrom = outfiles[i];

						// Get the duplicate status
						dupetype = file.GetDuplicateStatus(lastrom, logger);

						// If it's a duplicate, skip adding it to the output but add any missing information
						if (dupetype != 0x00)
						{
							// If we don't have a rom or disk, then just skip adding
							if (file.Type != ItemType.Rom && file.Type != ItemType.Disk)
							{
								continue;
							}

							saveditem = lastrom;
							pos = i;

							// Roms have more infomration to save
							if (file.Type == ItemType.Rom)
							{
								((Rom)saveditem).Size = ((Rom)saveditem).Size;
								((Rom)saveditem).CRC = (String.IsNullOrEmpty(((Rom)saveditem).CRC) && !String.IsNullOrEmpty(((Rom)file).CRC)
									? ((Rom)file).CRC
									: ((Rom)saveditem).CRC);
								((Rom)saveditem).MD5 = (String.IsNullOrEmpty(((Rom)saveditem).MD5) && !String.IsNullOrEmpty(((Rom)file).MD5)
									? ((Rom)file).MD5
									: ((Rom)saveditem).MD5);
								((Rom)saveditem).SHA1 = (String.IsNullOrEmpty(((Rom)saveditem).SHA1) && !String.IsNullOrEmpty(((Rom)file).SHA1)
									? ((Rom)file).SHA1
									: ((Rom)saveditem).SHA1);
							}
							else
							{
								((Disk)saveditem).MD5 = (String.IsNullOrEmpty(((Disk)saveditem).MD5) && !String.IsNullOrEmpty(((Disk)file).MD5)
									? ((Disk)file).MD5
									: ((Disk)saveditem).MD5);
								((Disk)saveditem).SHA1 = (String.IsNullOrEmpty(((Disk)saveditem).SHA1) && !String.IsNullOrEmpty(((Disk)file).SHA1)
									? ((Disk)file).SHA1
									: ((Disk)saveditem).SHA1);
							}

							saveditem.Dupe = dupetype;

							// If the current system has a lower ID than the previous, set the system accordingly
							if (file.SystemID < saveditem.SystemID)
							{
								saveditem.SystemID = file.SystemID;
								saveditem.System = file.System;
								saveditem.Machine.Name = file.Machine.Name;
								saveditem.Name = file.Name;
							}

							// If the current source has a lower ID than the previous, set the source accordingly
							if (file.SourceID < saveditem.SourceID)
							{
								saveditem.SourceID = file.SourceID;
								saveditem.Source = file.Source;
								saveditem.Machine.Name = file.Machine.Name;
								saveditem.Name = file.Name;
							}

							break;
						}
					}

					// If no duplicate is found, add it to the list
					if (dupetype == 0x00)
					{
						outfiles.Add(file);
					}
					// Otherwise, if a new rom information is found, add that
					else
					{
						outfiles.RemoveAt(pos);
						outfiles.Insert(pos, saveditem);
					}
				}
				else
				{
					outfiles.Add(file);
				}
			}

			// Then return the result
			return outfiles;
		}

		/// <summary>
		/// Sort a list of File objects by SystemID, SourceID, Game, and Name (in order)
		/// </summary>
		/// <param name="roms">List of File objects representing the roms to be sorted</param>
		/// <param name="norename">True if files are not renamed, false otherwise</param>
		/// <returns>True if it sorted correctly, false otherwise</returns>
		public static bool Sort(ref List<DatItem> roms, bool norename)
		{
			try
			{
				roms.Sort(delegate (DatItem x, DatItem y)
				{
					NaturalComparer nc = new NaturalComparer();
					if (x.SystemID == y.SystemID)
					{
						if (x.SourceID == y.SourceID)
						{
							if (x.Machine.Name == y.Machine.Name)
							{
								if ((x.Type == ItemType.Rom || x.Type == ItemType.Disk) && (y.Type == ItemType.Rom || y.Type == ItemType.Disk))
								{
									if (Path.GetDirectoryName(Style.RemovePathUnsafeCharacters(x.Name)) == Path.GetDirectoryName(Style.RemovePathUnsafeCharacters(y.Name)))
									{
										return nc.Compare(Path.GetFileName(Style.RemovePathUnsafeCharacters(x.Name)), Path.GetFileName(Style.RemovePathUnsafeCharacters(y.Name)));
									}
									return nc.Compare(Path.GetDirectoryName(Style.RemovePathUnsafeCharacters(x.Name)), Path.GetDirectoryName(Style.RemovePathUnsafeCharacters(y.Name)));
								}
								else if ((x.Type == ItemType.Rom || x.Type == ItemType.Disk) && (y.Type != ItemType.Rom && y.Type != ItemType.Disk))
								{
									return -1;
								}
								else if ((x.Type != ItemType.Rom && x.Type != ItemType.Disk) && (y.Type == ItemType.Rom || y.Type == ItemType.Disk))
								{
									return 1;
								}
								else
								{
									if (Path.GetDirectoryName(x.Name) == Path.GetDirectoryName(y.Name))
									{
										return nc.Compare(Path.GetFileName(x.Name), Path.GetFileName(y.Name));
									}
									return nc.Compare(Path.GetDirectoryName(x.Name), Path.GetDirectoryName(y.Name));
								}
							}
							return nc.Compare(x.Machine.Name, y.Machine.Name);
						}
						return (norename ? nc.Compare(x.Machine.Name, y.Machine.Name) : x.SourceID - y.SourceID);
					}
					return (norename ? nc.Compare(x.Machine.Name, y.Machine.Name) : x.SystemID - y.SystemID);
				});
				return true;
			}
			catch (Exception)
			{
				// Absorb the error
				return false;
			}
		}

		#endregion

		#endregion // Static Methods
	}
}