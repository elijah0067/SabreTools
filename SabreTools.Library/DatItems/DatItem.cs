﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Library.Data;
using SabreTools.Library.FileTypes;
using SabreTools.Library.Filtering;
using SabreTools.Library.Logging;
using SabreTools.Library.Tools;
using NaturalSort;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Base class for all items included in a set
    /// </summary>
    [JsonObject("datitem"), XmlRoot("datitem")]
    [XmlInclude(typeof(Adjuster))]
    [XmlInclude(typeof(Analog))]
    [XmlInclude(typeof(Archive))]
    [XmlInclude(typeof(BiosSet))]
    [XmlInclude(typeof(Blank))]
    [XmlInclude(typeof(Chip))]
    [XmlInclude(typeof(Condition))]
    [XmlInclude(typeof(Configuration))]
    [XmlInclude(typeof(Control))]
    [XmlInclude(typeof(DataArea))]
    [XmlInclude(typeof(Device))]
    [XmlInclude(typeof(DeviceReference))]
    [XmlInclude(typeof(DipSwitch))]
    [XmlInclude(typeof(Disk))]
    [XmlInclude(typeof(DiskArea))]
    [XmlInclude(typeof(Display))]
    [XmlInclude(typeof(Driver))]
    [XmlInclude(typeof(Extension))]
    [XmlInclude(typeof(Feature))]
    [XmlInclude(typeof(Info))]
    [XmlInclude(typeof(Input))]
    [XmlInclude(typeof(Instance))]
    [XmlInclude(typeof(Location))]
    [XmlInclude(typeof(Media))]
    [XmlInclude(typeof(Part))]
    [XmlInclude(typeof(PartFeature))]
    [XmlInclude(typeof(Port))]
    [XmlInclude(typeof(RamOption))]
    [XmlInclude(typeof(Release))]
    [XmlInclude(typeof(Rom))]
    [XmlInclude(typeof(Sample))]
    [XmlInclude(typeof(Setting))]
    [XmlInclude(typeof(SharedFeature))]
    [XmlInclude(typeof(Slot))]
    [XmlInclude(typeof(SlotOption))]
    [XmlInclude(typeof(SoftwareList))]
    [XmlInclude(typeof(Sound))]
    public abstract class DatItem : IEquatable<DatItem>, IComparable<DatItem>, ICloneable
    {
        #region Fields

        #region Common Fields

        /// <summary>
        /// Item type for outputting
        /// </summary>
        [JsonProperty("itemtype")]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("itemtype")]
        public ItemType ItemType { get; set; }

        /// <summary>
        /// Duplicate type when compared to another item
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public DupeType DupeType { get; set; }

        #endregion

        #region Machine Fields

        /// <summary>
        /// Machine values
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public Machine Machine { get; set; } = new Machine();

        #endregion

        #region Metadata information

        /// <summary>
        /// Source information
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public Source Source { get; set; } = new Source();

        /// <summary>
        /// Flag if item should be removed
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public bool Remove { get; set; }

        #endregion

        #region Static Values

        /// <summary>
        /// Fields unique to a DatItem
        /// </summary>
        public static readonly List<Field> DatItemFields = new List<Field>()
        {
            #region Common

            Field.DatItem_Type,

            #endregion

            #region Item-Specific

            #region Actionable

            // Rom
            Field.DatItem_Name,
            Field.DatItem_Bios,
            Field.DatItem_Size,
            Field.DatItem_CRC,
            Field.DatItem_MD5,
#if NET_FRAMEWORK
            Field.DatItem_RIPEMD160,
#endif
            Field.DatItem_SHA1,
            Field.DatItem_SHA256,
            Field.DatItem_SHA384,
            Field.DatItem_SHA512,
            Field.DatItem_SpamSum,
            Field.DatItem_Merge,
            Field.DatItem_Region,
            Field.DatItem_Offset,
            Field.DatItem_Date,
            Field.DatItem_Status,
            Field.DatItem_Optional,
            Field.DatItem_Inverted,

            // Rom (AttractMode)
            Field.DatItem_AltName,
            Field.DatItem_AltTitle,

            // Rom (OpenMSX)
            Field.DatItem_Original,
            Field.DatItem_OpenMSXSubType,
            Field.DatItem_OpenMSXType,
            Field.DatItem_Remark,
            Field.DatItem_Boot,

            // Rom (SoftwareList)
            Field.DatItem_LoadFlag,
            Field.DatItem_Value,

            // Disk
            Field.DatItem_Index,
            Field.DatItem_Writable,

            #endregion

            #region Auxiliary

            // Adjuster
            Field.DatItem_Default,

            // Analog
            Field.DatItem_Analog_Mask,

            // BiosSet
            Field.DatItem_Description,

            // Chip
            Field.DatItem_Tag,
            Field.DatItem_ChipType,
            Field.DatItem_Clock,

            // Condition
            Field.DatItem_Mask,
            Field.DatItem_Relation,
            Field.DatItem_Condition_Tag,
            Field.DatItem_Condition_Mask,
            Field.DatItem_Condition_Relation,
            Field.DatItem_Condition_Value,

            // Control
            Field.DatItem_Control_Type,
            Field.DatItem_Control_Player,
            Field.DatItem_Control_Buttons,
            Field.DatItem_Control_RequiredButtons,
            Field.DatItem_Control_Minimum,
            Field.DatItem_Control_Maximum,
            Field.DatItem_Control_Sensitivity,
            Field.DatItem_Control_KeyDelta,
            Field.DatItem_Control_Reverse,
            Field.DatItem_Control_Ways,
            Field.DatItem_Control_Ways2,
            Field.DatItem_Control_Ways3,

            // DataArea
            Field.DatItem_AreaName,
            Field.DatItem_AreaSize,
            Field.DatItem_AreaWidth,
            Field.DatItem_AreaEndianness,

            // Device
            Field.DatItem_DeviceType,
            Field.DatItem_FixedImage,
            Field.DatItem_Mandatory,
            Field.DatItem_Interface,

            // Display
            Field.DatItem_DisplayType,
            Field.DatItem_Rotate,
            Field.DatItem_FlipX,
            Field.DatItem_Width,
            Field.DatItem_Height,
            Field.DatItem_Refresh,
            Field.DatItem_PixClock,
            Field.DatItem_HTotal,
            Field.DatItem_HBEnd,
            Field.DatItem_HBStart,
            Field.DatItem_VTotal,
            Field.DatItem_VBEnd,
            Field.DatItem_VBStart,

            // Driver
            Field.DatItem_SupportStatus,
            Field.DatItem_EmulationStatus,
            Field.DatItem_CocktailStatus,
            Field.DatItem_SaveStateStatus,

            // Extension
            Field.DatItem_Extension_Name,

            // Feature
            Field.DatItem_FeatureType,
            Field.DatItem_FeatureStatus,
            Field.DatItem_FeatureOverall,

            // Input
            Field.DatItem_Service,
            Field.DatItem_Tilt,
            Field.DatItem_Players,
            Field.DatItem_Coins,

            // Instance
            Field.DatItem_Instance_Name,
            Field.DatItem_Instance_BriefName,

            // Location
            Field.DatItem_Location_Name,
            Field.DatItem_Location_Number,
            Field.DatItem_Location_Inverted,

            // Part
            Field.DatItem_Part_Name,
            Field.DatItem_Part_Interface,

            // PartFeature
            Field.DatItem_Part_Feature_Name,
            Field.DatItem_Part_Feature_Value,

            // RamOption
            Field.DatItem_Content,

            // Release
            Field.DatItem_Language,

            // Setting
            Field.DatItem_Setting_Name,
            Field.DatItem_Setting_Value,
            Field.DatItem_Setting_Default,

            // SlotOption
            Field.DatItem_SlotOption_Name,
            Field.DatItem_SlotOption_DeviceName,
            Field.DatItem_SlotOption_Default,

            // SoftwareList
            Field.DatItem_SoftwareListStatus,
            Field.DatItem_Filter,

            // Sound
            Field.DatItem_Channels,

            #endregion

            #endregion // Item-Specific
        };

        /// <summary>
        /// Fields unique to a Machine
        /// </summary>
        public static readonly List<Field> MachineFields = new List<Field>()
        {
            #region Common

            Field.Machine_Name,
            Field.Machine_Comment,
            Field.Machine_Description,
            Field.Machine_Year,
            Field.Machine_Manufacturer,
            Field.Machine_Publisher,
            Field.Machine_Category,
            Field.Machine_RomOf,
            Field.Machine_CloneOf,
            Field.Machine_SampleOf,
            Field.Machine_Type,

            #endregion

            #region AttractMode

            Field.Machine_Players,
            Field.Machine_Rotation,
            Field.Machine_Control,
            Field.Machine_Status,
            Field.Machine_DisplayCount,
            Field.Machine_DisplayType,
            Field.Machine_Buttons,

            #endregion

            #region ListXML

            Field.Machine_SourceFile,
            Field.Machine_Runnable,

            #endregion

            #region Logiqx

            Field.Machine_Board,
            Field.Machine_RebuildTo,

            #endregion

            #region Logiqx EmuArc

            Field.Machine_TitleID,
            Field.Machine_Developer,
            Field.Machine_Genre,
            Field.Machine_Subgenre,
            Field.Machine_Ratings,
            Field.Machine_Score,
            Field.Machine_Enabled,
            Field.Machine_CRC,
            Field.Machine_RelatedTo,

            #endregion

            #region OpenMSX

            Field.Machine_GenMSXID,
            Field.Machine_System,
            Field.Machine_Country,

            #endregion

            #region SoftwareList

            Field.Machine_Supported,

            #endregion
        };

        #endregion

        #endregion
        
        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        [JsonIgnore, XmlIgnore]
        protected Logger logger;

        /// <summary>
        /// Static logger for static methods
        /// </summary>
        [JsonIgnore, XmlIgnore]
        protected static Logger staticLogger = new Logger();

        #endregion

        #region Instance Methods

        #region Accessors

        /// <summary>
        /// Gets the name to use for a DatItem
        /// </summary>
        /// <returns>Name if available, null otherwise</returns>
        public virtual string GetName()
        {
            return null;
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public virtual void SetFields(Dictionary<Field, string> mappings)
        {
            // Set machine fields
            if (Machine == null)
                Machine = new Machine();

            Machine.SetFields(mappings);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public DatItem()
        {
            logger = new Logger(this);
        }

        /// <summary>
        /// Create a specific type of DatItem to be used based on an ItemType
        /// </summary>
        /// <param name="itemType">Type of the DatItem to be created</param>
        /// <returns>DatItem of the specific internal type that corresponds to the inputs</returns>
        public static DatItem Create(ItemType? itemType)
        {
#if NET_FRAMEWORK
            switch (itemType)
            {
                case ItemType.Adjuster:
                    return new Adjuster();

                case ItemType.Analog:
                    return new Analog();

                case ItemType.Archive:
                    return new Archive();

                case ItemType.BiosSet:
                    return new BiosSet();

                case ItemType.Blank:
                    return new Blank();

                case ItemType.Chip:
                    return new Chip();

                case ItemType.Condition:
                    return new Condition();

                case ItemType.Configuration:
                    return new Configuration();
                
                case ItemType.Control:
                    return new Control();

                case ItemType.DataArea:
                    return new DataArea();

                case ItemType.Device:
                    return new Device();

                case ItemType.DeviceReference:
                    return new DeviceReference();

                case ItemType.DipSwitch:
                    return new DipSwitch();

                case ItemType.Disk:
                    return new Disk();

                case ItemType.DiskArea:
                    return new DiskArea();

                case ItemType.Display:
                    return new Display();

                case ItemType.Driver:
                    return new Driver();

                case ItemType.Extension:
                    return new Extension();

                case ItemType.Feature:
                    return new Feature();

                case ItemType.Info:
                    return new Info();

                case ItemType.Input:
                    return new Input();

                case ItemType.Instance:
                    return new Instance();

                case ItemType.Location:
                    return new Location();

                case ItemType.Media:
                    return new Media();

                case ItemType.Part:
                    return new Part();

                case ItemType.PartFeature:
                    return new PartFeature();

                case ItemType.Port:
                    return new Port();

                case ItemType.RamOption:
                    return new RamOption();

                case ItemType.Release:
                    return new Release();

                case ItemType.Rom:
                    return new Rom();

                case ItemType.Sample:
                    return new Sample();

                case ItemType.Setting:
                    return new Setting();

                case ItemType.SharedFeature:
                    return new SharedFeature();

                case ItemType.Slot:
                    return new Slot();

                case ItemType.SlotOption:
                    return new SlotOption();

                case ItemType.SoftwareList:
                    return new SoftwareList();

                case ItemType.Sound:
                    return new Sound();

                default:
                    return new Rom();
            }
#else
            return itemType switch
            {
                ItemType.Adjuster => new Adjuster(),
                ItemType.Analog => new Analog(),
                ItemType.Archive => new Archive(),
                ItemType.BiosSet => new BiosSet(),
                ItemType.Blank => new Blank(),
                ItemType.Chip => new Chip(),
                ItemType.Condition => new Condition(),
                ItemType.Configuration => new Configuration(),
                ItemType.Device => new Device(),
                ItemType.DeviceReference => new DeviceReference(),
                ItemType.DipSwitch => new DipSwitch(),
                ItemType.Disk => new Disk(),
                ItemType.Display => new Display(),
                ItemType.Driver => new Driver(),
                ItemType.Extension => new Extension(),
                ItemType.Feature => new Feature(),
                ItemType.Info => new Info(),
                ItemType.Instance => new Instance(),
                ItemType.Location => new Location(),
                ItemType.Media => new Media(),
                ItemType.PartFeature => new PartFeature(),
                ItemType.Port => new Port(),
                ItemType.RamOption => new RamOption(),
                ItemType.Release => new Release(),
                ItemType.Rom => new Rom(),
                ItemType.Sample => new Sample(),
                ItemType.SharedFeature => new SharedFeature(),
                ItemType.Slot => new Slot(),
                ItemType.SlotOption => new SlotOption(),
                ItemType.SoftwareList => new SoftwareList(),
                ItemType.Sound => new Sound(),
                _ => new Rom(),
            };
#endif
        }

        /// <summary>
        /// Create a specific type of DatItem to be used based on a BaseFile
        /// </summary>
        /// <param name="baseFile">BaseFile containing information to be created</param>
        /// <returns>DatItem of the specific internal type that corresponds to the inputs</returns>
        public static DatItem Create(BaseFile baseFile)
        {
            switch (baseFile.Type)
            {
                case FileType.AaruFormat:
                    return new Media(baseFile);

                case FileType.CHD:
                    return new Disk(baseFile);

                case FileType.GZipArchive:
                case FileType.LRZipArchive:
                case FileType.LZ4Archive:
                case FileType.None:
                case FileType.RarArchive:
                case FileType.SevenZipArchive:
                case FileType.TapeArchive:
                case FileType.XZArchive:
                case FileType.ZipArchive:
                case FileType.ZPAQArchive:
                case FileType.ZstdArchive:
                    return new Rom(baseFile);

                case FileType.Folder:
                default:
                    return null;
            }
        }

        #endregion

        #region Cloning Methods

        /// <summary>
        /// Clone the DatItem
        /// </summary>
        /// <returns>Clone of the DatItem</returns>
        public abstract object Clone();

        /// <summary>
        /// Copy all machine information over in one shot
        /// </summary>
        /// <param name="item">Existing item to copy information from</param>
        public void CopyMachineInformation(DatItem item)
        {
            Machine = (Machine)item.Machine.Clone();
        }

        /// <summary>
        /// Copy all machine information over in one shot
        /// </summary>
        /// <param name="machine">Existing machine to copy information from</param>
        public void CopyMachineInformation(Machine machine)
        {
            Machine = (Machine)machine.Clone();
        }

        #endregion

        #region Comparision Methods

        public int CompareTo(DatItem other)
        {
            try
            {
                if (GetName() == other.GetName())
                    return Equals(other) ? 0 : 1;

                return string.Compare(GetName(), other.GetName());
            }
            catch
            {
                return 1;
            }
        }

        /// <summary>
        /// Determine if an item is a duplicate using partial matching logic
        /// </summary>
        /// <param name="other">DatItem to use as a baseline</param>
        /// <returns>True if the roms are duplicates, false otherwise</returns>
        public abstract bool Equals(DatItem other);

        /// <summary>
        /// Return the duplicate status of two items
        /// </summary>
        /// <param name="lastItem">DatItem to check against</param>
        /// <returns>The DupeType corresponding to the relationship between the two</returns>
        public DupeType GetDuplicateStatus(DatItem lastItem)
        {
            DupeType output = 0x00;

            // If we don't have a duplicate at all, return none
            if (!Equals(lastItem))
                return output;

            // If the duplicate is external already or should be, set it
            if (lastItem.DupeType.HasFlag(DupeType.External) || lastItem.Source.Index != Source.Index)
            {
                if (lastItem.Machine.Name == Machine.Name && lastItem.GetName() == GetName())
                    output = DupeType.External | DupeType.All;
                else
                    output = DupeType.External | DupeType.Hash;
            }

            // Otherwise, it's considered an internal dupe
            else
            {
                if (lastItem.Machine.Name == Machine.Name && lastItem.GetName() == GetName())
                    output = DupeType.Internal | DupeType.All;
                else
                    output = DupeType.Internal | DupeType.Hash;
            }

            return output;
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Clean a DatItem according to the cleaner
        /// </summary>
        /// <param name="cleaner">Cleaner to implement</param>
        public virtual void Clean(Cleaner cleaner)
        {
            // If we're stripping unicode characters, strip machine name and description
            if (cleaner?.RemoveUnicode == true)
            {
                Machine.Name = Sanitizer.RemoveUnicodeCharacters(Machine.Name);
                Machine.Description = Sanitizer.RemoveUnicodeCharacters(Machine.Description);
            }

            // If we're in cleaning mode, sanitize machine name and description
            if (cleaner?.Clean == true)
            {
                Machine.Name = Sanitizer.CleanGameName(Machine.Name);
                Machine.Description = Sanitizer.CleanGameName(Machine.Description);
            }

            // If we are in single game mode, rename the machine
            if (cleaner?.Single == true)
                Machine.Name = "!";
        }

        /// <summary>
        /// Check to see if a DatItem passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <param name="sub">True if this is a subitem, false otherwise</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public virtual bool PassesFilter(Filter filter, bool sub = false)
        {
            // Filter on machine fields
            if (!Machine.PassesFilter(filter))
                return false;

            // Filters for if we're a top-level item
            if (!sub)
            {
                // Filter on item type
                if (!filter.PassStringFilter(filter.DatItem_Type, ItemType.ToString()))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Remove fields from the DatItem
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        public virtual void RemoveFields(List<Field> fields)
        {
            // Remove machine fields
            Machine.RemoveFields(fields);
        }

        /// <summary>
        /// Set internal names to match One Rom Per Game (ORPG) logic
        /// </summary>
        public virtual void SetOneRomPerGame()
        {
        }

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Get the dictionary key that should be used for a given item and bucketing type
        /// </summary>
        /// <param name="bucketedBy">Field value representing what key to get</param>
        /// <param name="lower">True if the key should be lowercased (default), false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        /// <returns>String representing the key to be used for the DatItem</returns>
        /// TODO: What other fields can we reasonably allow bucketing on?
        public virtual string GetKey(Field bucketedBy, bool lower = true, bool norename = true)
        {
            // Set the output key as the default blank string
            string key = string.Empty;

            // Now determine what the key should be based on the bucketedBy value
            switch (bucketedBy)
            {
                case Field.DatItem_CRC:
                    key = Constants.CRCZero;
                    break;

                case Field.Machine_Name:
                    key = (norename ? string.Empty
                        : Source.Index.ToString().PadLeft(10, '0')
                            + "-")
                    + (string.IsNullOrWhiteSpace(Machine.Name)
                            ? "Default"
                            : Machine.Name);
                    if (lower)
                        key = key.ToLowerInvariant();

                    if (key == null)
                        key = "null";

                    break;

                case Field.DatItem_MD5:
                    key = Constants.MD5Zero;
                    break;

#if NET_FRAMEWORK
                case Field.DatItem_RIPEMD160:
                    key = Constants.RIPEMD160Zero;
                    break;
#endif

                case Field.DatItem_SHA1:
                    key = Constants.SHA1Zero;
                    break;

                case Field.DatItem_SHA256:
                    key = Constants.SHA256Zero;
                    break;

                case Field.DatItem_SHA384:
                    key = Constants.SHA384Zero;
                    break;

                case Field.DatItem_SHA512:
                    key = Constants.SHA512Zero;
                    break;

                case Field.DatItem_SpamSum:
                    key = Constants.SpamSumZero;
                    break;
            }

            // Double and triple check the key for corner cases
            if (key == null)
                key = string.Empty;

            return key;
        }

        /// <summary>
        /// Replace fields from another item
        /// </summary>
        /// <param name="item">DatItem to pull new information from</param>
        /// <param name="fields">List of Fields representing what should be updated</param>
        public virtual void ReplaceFields(DatItem item, List<Field> fields)
        {
        }

        #endregion

        #endregion // Instance Methods

        #region Static Methods

        #region Sorting and Merging

        /// <summary>
        /// Determine if two hashes are equal for the purposes of merging
        /// </summary>
        /// <param name="firstHash">First hash to compare</param>
        /// <param name="secondHash">Second hash to compare</param>
        /// <returns>True if either is empty OR the hashes exactly match, false otherwise</returns>
        public static bool ConditionalHashEquals(byte[] firstHash, byte[] secondHash)
        {
            // If either hash is empty, we say they're equal for merging
            if (firstHash.IsNullOrEmpty() || secondHash.IsNullOrEmpty())
                return true;

            // If they're different sizes, they can't match
            if (firstHash.Length != secondHash.Length)
                return false;

            // Otherwise, they need to match exactly
            return Enumerable.SequenceEqual(firstHash, secondHash);
        }

        /// <summary>
        /// Merge an arbitrary set of ROMs based on the supplied information
        /// </summary>
        /// <param name="infiles">List of File objects representing the roms to be merged</param>
        /// <returns>A List of DatItem objects representing the merged roms</returns>
        public static List<DatItem> Merge(List<DatItem> infiles)
        {
            // Check for null or blank roms first
            if (infiles == null || infiles.Count == 0)
                return new List<DatItem>();

            // Create output list
            List<DatItem> outfiles = new List<DatItem>();

            // Then deduplicate them by checking to see if data matches previous saved roms
            int nodumpCount = 0;
            for (int f = 0; f < infiles.Count; f++)
            {
                DatItem file = infiles[f];

                // If we somehow have a null item, skip
                if (file == null)
                    continue;

                // If we don't have a Dis, Media, or Rom, we skip checking for duplicates
                if (file.ItemType != ItemType.Disk && file.ItemType != ItemType.Media && file.ItemType != ItemType.Rom)
                    continue;

                // If it's a nodump, add and skip
                if (file.ItemType == ItemType.Rom && (file as Rom).ItemStatus == ItemStatus.Nodump)
                {
                    outfiles.Add(file);
                    nodumpCount++;
                    continue;
                }
                else if (file.ItemType == ItemType.Disk && (file as Disk).ItemStatus == ItemStatus.Nodump)
                {
                    outfiles.Add(file);
                    nodumpCount++;
                    continue;
                }
                // If it's the first non-nodump rom in the list, don't touch it
                else if (outfiles.Count == 0 || outfiles.Count == nodumpCount)
                {
                    outfiles.Add(file);
                    continue;
                }

                // Check if the rom is a duplicate
                DupeType dupetype = 0x00;
                DatItem saveditem = new Blank();
                int pos = -1;
                for (int i = 0; i < outfiles.Count; i++)
                {
                    DatItem lastrom = outfiles[i];

                    // Get the duplicate status
                    dupetype = file.GetDuplicateStatus(lastrom);

                    // If it's a duplicate, skip adding it to the output but add any missing information
                    if (dupetype != 0x00)
                    {
                        saveditem = lastrom;
                        pos = i;

                        // Disks, Media, and Roms have more information to fill
                        if (file.ItemType == ItemType.Disk)
                            (saveditem as Disk).FillMissingInformation(file as Disk);
                        else if (file.ItemType == ItemType.Media)
                            (saveditem as Media).FillMissingInformation(file as Media);
                        else if (file.ItemType == ItemType.Rom)
                            (saveditem as Rom).FillMissingInformation(file as Rom);

                        saveditem.DupeType = dupetype;

                        // If the current system has a lower ID than the previous, set the system accordingly
                        if (file.Source.Index < saveditem.Source.Index)
                        {
                            saveditem.Source = file.Source.Clone() as Source;
                            saveditem.CopyMachineInformation(file);
                            saveditem.SetFields(new Dictionary<Field, string> { [Field.DatItem_Name] = file.GetName() });
                        }

                        // If the current machine is a child of the new machine, use the new machine instead
                        if (saveditem.Machine.CloneOf == file.Machine.Name || saveditem.Machine.RomOf == file.Machine.Name)
                        {
                            saveditem.CopyMachineInformation(file);
                            saveditem.SetFields(new Dictionary<Field, string> { [Field.DatItem_Name] = file.GetName() });
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

            // Then return the result
            return outfiles;
        }

        /// <summary>
        /// Resolve name duplicates in an arbitrary set of ROMs based on the supplied information
        /// </summary>
        /// <param name="infiles">List of File objects representing the roms to be merged</param>
        /// <returns>A List of DatItem objects representing the renamed roms</returns>
        public static List<DatItem> ResolveNames(List<DatItem> infiles)
        {
            // Create the output list
            List<DatItem> output = new List<DatItem>();

            // First we want to make sure the list is in alphabetical order
            Sort(ref infiles, true);

            // Now we want to loop through and check names
            DatItem lastItem = null;
            string lastrenamed = null;
            int lastid = 0;
            for (int i = 0; i < infiles.Count; i++)
            {
                DatItem datItem = infiles[i];

                // If we have the first item, we automatically add it
                if (lastItem == null)
                {
                    output.Add(datItem);
                    lastItem = datItem;
                    continue;
                }

                // Get the last item name, if applicable
                string lastItemName = lastItem.GetName() ?? lastItem.ItemType.ToString();

                // Get the current item name, if applicable
                string datItemName = datItem.GetName() ?? datItem.ItemType.ToString();

                // If the current item exactly matches the last item, then we don't add it
                if (datItem.GetDuplicateStatus(lastItem).HasFlag(DupeType.All))
                {
                    staticLogger.Verbose($"Exact duplicate found for '{datItemName}'");
                    continue;
                }

                // If the current name matches the previous name, rename the current item
                else if (datItemName == lastItemName)
                {
                    staticLogger.Verbose($"Name duplicate found for '{datItemName}'");

                    if (datItem.ItemType == ItemType.Disk || datItem.ItemType == ItemType.Media || datItem.ItemType == ItemType.Rom)
                    {
                        datItemName += GetDuplicateSuffix(datItem);
#if NET_FRAMEWORK
                        lastrenamed = lastrenamed ?? datItemName;
#else
                        lastrenamed ??= datItemName;
#endif
                    }

                    // If we have a conflict with the last renamed item, do the right thing
                    if (datItemName == lastrenamed)
                    {
                        lastrenamed = datItemName;
                        datItemName += (lastid == 0 ? string.Empty : "_" + lastid);
                        lastid++;
                    }
                    // If we have no conflict, then we want to reset the lastrenamed and id
                    else
                    {
                        lastrenamed = null;
                        lastid = 0;
                    }

                    // Set the item name back to the datItem
                    datItem.SetFields(new Dictionary<Field, string> { [Field.DatItem_Name] = datItemName });

                    output.Add(datItem);
                }

                // Otherwise, we say that we have a valid named file
                else
                {
                    output.Add(datItem);
                    lastItem = datItem;
                    lastrenamed = null;
                    lastid = 0;
                }
            }

            // One last sort to make sure this is ordered
            Sort(ref output, true);

            return output;
        }

        /// <summary>
        /// Get duplicate suffix based on the item type
        /// </summary>
        private static string GetDuplicateSuffix(DatItem datItem)
        {
            if (datItem.ItemType == ItemType.Disk)
                return (datItem as Disk).GetDuplicateSuffix();
            else if (datItem.ItemType == ItemType.Media)
                return (datItem as Media).GetDuplicateSuffix();
            else if (datItem.ItemType == ItemType.Rom)
                return (datItem as Rom).GetDuplicateSuffix();

            return "_1";
        }

        /// <summary>
        /// Sort a list of File objects by SourceID, Game, and Name (in order)
        /// </summary>
        /// <param name="roms">List of File objects representing the roms to be sorted</param>
        /// <param name="norename">True if files are not renamed, false otherwise</param>
        /// <returns>True if it sorted correctly, false otherwise</returns>
        public static bool Sort(ref List<DatItem> roms, bool norename)
        {
            roms.Sort(delegate (DatItem x, DatItem y)
            {
                try
                {
                    NaturalComparer nc = new NaturalComparer();
                    if (x.Source.Index == y.Source.Index)
                    {
                        if (x.Machine.Name == y.Machine.Name)
                        {
                            if (x.ItemType == y.ItemType)
                            {
                                if (Path.GetDirectoryName(Sanitizer.RemovePathUnsafeCharacters(x.GetName() ?? string.Empty)) == Path.GetDirectoryName(Sanitizer.RemovePathUnsafeCharacters(y.GetName() ?? string.Empty)))
                                    return nc.Compare(Path.GetFileName(Sanitizer.RemovePathUnsafeCharacters(x.GetName() ?? string.Empty)), Path.GetFileName(Sanitizer.RemovePathUnsafeCharacters(y.GetName() ?? string.Empty)));

                                return nc.Compare(Path.GetDirectoryName(Sanitizer.RemovePathUnsafeCharacters(x.GetName() ?? string.Empty)), Path.GetDirectoryName(Sanitizer.RemovePathUnsafeCharacters(y.GetName() ?? string.Empty)));
                            }

                            return x.ItemType - y.ItemType;
                        }

                        return nc.Compare(x.Machine.Name, y.Machine.Name);
                    }

                    return (norename ? nc.Compare(x.Machine.Name, y.Machine.Name) : x.Source.Index - y.Source.Index);
                }
                catch (Exception ex)
                {
                    // Absorb the error
                    return 0;
                }
            });

            return true;
        }

        #endregion

        #endregion // Static Methods
    }
}
