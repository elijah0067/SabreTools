﻿using System;
using System.Collections.Generic;
using System.Linq;

using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents the information specific to a set/game/machine
    /// </summary>
    [JsonObject("machine")]
    public class Machine : ICloneable
    {
        #region Fields

        #region Common Fields

        /// <summary>
        /// Name of the machine
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Name { get; set; } = null;

        /// <summary>
        /// Additional notes
        /// </summary>
        /// <remarks>Known as "Extra" in AttractMode</remarks>
        [JsonProperty("comment", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Comment { get; set; } = null;

        /// <summary>
        /// Extended description
        /// </summary>
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Description { get; set; } = null;

        /// <summary>
        /// Year(s) of release/manufacture
        /// </summary>
        [JsonProperty("year", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Year { get; set; } = null;

        /// <summary>
        /// Manufacturer, if available
        /// </summary>
        [JsonProperty("manufacturer", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Manufacturer { get; set; } = null;

        /// <summary>
        /// Publisher, if available
        /// </summary>
        [JsonProperty("publisher", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Publisher { get; set; } = null;

        /// <summary>
        /// Category, if available
        /// </summary>
        [JsonProperty("category", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Category { get; set; } = null;

        /// <summary>
        /// fomof parent
        /// </summary>
        [JsonProperty("romof", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RomOf { get; set; } = null;

        /// <summary>
        /// cloneof parent
        /// </summary>
        [JsonProperty("cloneof", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CloneOf { get; set; } = null;

        /// <summary>
        /// sampleof parent
        /// </summary>
        [JsonProperty("sampleof", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SampleOf { get; set; } = null;

        /// <summary>
        /// Type of the machine
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MachineType MachineType { get; set; } = MachineType.NULL;

        #endregion

        #region AttractMode Fields

        /// <summary>
        /// Player count
        /// </summary>
        /// <remarks>Also in Logiqx EmuArc</remarks>
        [JsonProperty("players", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Players { get; set; } = null;

        /// <summary>
        /// Screen rotation
        /// </summary>
        [JsonProperty("rotation", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Rotation { get; set; } = null;

        /// <summary>
        /// Control method
        /// </summary>
        [JsonProperty("control", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Control { get; set; } = null;

        /// <summary>
        /// Support status
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Status { get; set; } = null;

        /// <summary>
        /// Display count
        /// </summary>
        [JsonProperty("displaycount", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string DisplayCount { get; set; } = null;

        /// <summary>
        /// Display type
        /// </summary>
        [JsonProperty("displaytype", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string DisplayType { get; set; } = null;

        /// <summary>
        /// Number of input buttons
        /// </summary>
        [JsonProperty("buttons", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Buttons { get; set; } = null;

        #endregion

        #region ListXML Fields

        /// <summary>
        /// Emulator source file related to the machine
        /// </summary>
        /// <remarks>Also in Logiqx</remarks>
        [JsonProperty("sourcefile", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SourceFile { get; set; } = null;

        /// <summary>
        /// Machine runnable status
        /// </summary>
        /// <remarks>Also in Logiqx</remarks>
        [JsonProperty("runnable", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Runnable Runnable { get; set; } = Runnable.NULL;

        /// <summary>
        /// List of associated device names
        /// </summary>
        [JsonProperty("devices", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlDeviceReference> DeviceReferences { get; set; } = null;

        /// <summary>
        /// List of associated chips
        /// </summary>
        [JsonProperty("chips", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlChip> Chips { get; set; } = null;

        /// <summary>
        /// List of associated displays
        /// </summary>
        [JsonProperty("displays", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlDisplay> Displays { get; set; } = null;

        /// <summary>
        /// List of associated sounds
        /// </summary>
        [JsonProperty("sounds", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlSound> Sounds { get; set; } = null;

        /// <summary>
        /// List of associated conditions
        /// </summary>
        [JsonProperty("conditions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlCondition> Conditions { get; set; } = null;

        /// <summary>
        /// List of associated inputs
        /// </summary>
        [JsonProperty("inputs", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlInput> Inputs { get; set; } = null;

        /// <summary>
        /// List of associated dipswitches
        /// </summary>
        /// <remarks>Also in SoftwareList</remarks>
        /// TODO: Order ListXML and SoftwareList outputs by area names
        [JsonProperty("dipswitches", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlDipSwitch> DipSwitches { get; set; } = null;

        /// <summary>
        /// List of associated configurations
        /// </summary>
        [JsonProperty("configurations", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlConfiguration> Configurations { get; set; } = null;

        /// <summary>
        /// List of associated ports
        /// </summary>
        [JsonProperty("ports", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlPort> Ports { get; set; } = null;

        /// <summary>
        /// List of associated adjusters
        /// </summary>
        [JsonProperty("adjusters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlAdjuster> Adjusters { get; set; } = null;

        /// <summary>
        /// List of associated drivers
        /// </summary>
        [JsonProperty("drivers", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlDriver> Drivers { get; set; } = null;

        /// <summary>
        /// List of associated features
        /// </summary>
        [JsonProperty("features", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlFeature> Features { get; set; } = null;

        /// <summary>
        /// List of associated devices
        /// </summary>
        [JsonProperty("devices", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlDevice> Devices { get; set; } = null;

        /// <summary>
        /// List of slot options
        /// </summary>
        [JsonProperty("slots", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlSlot> Slots { get; set; } = null;

        /// <summary>
        /// List of software lists
        /// </summary>
        [JsonProperty("softwarelists", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlSoftwareList> SoftwareLists { get; set; } = null;

        /// <summary>
        /// List of ramoptions
        /// </summary>
        [JsonProperty("ramoptions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlRamOption> RamOptions { get; set; } = null;

        #endregion

        #region Logiqx Fields

        /// <summary>
        /// Machine board name
        /// </summary>
        [JsonProperty("board", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Board { get; set; } = null;

        /// <summary>
        /// Rebuild location if different than machine name
        /// </summary>
        [JsonProperty("rebuildto", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RebuildTo { get; set; } = null;

        #endregion

        // TODO: Should this be a separate object for TruRip?
        #region Logiqx EmuArc Fields

        /// <summary>
        /// Title ID
        /// </summary>
        [JsonProperty("titleid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string TitleID { get; set; } = null;

        /// <summary>
        /// Machine developer
        /// </summary>
        [JsonProperty("developer", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Developer { get; set; } = null;

        /// <summary>
        /// Game genre
        /// </summary>
        [JsonProperty("genre", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Genre { get; set; } = null;

        /// <summary>
        /// Game subgenre
        /// </summary>
        [JsonProperty("subgenre", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Subgenre { get; set; } = null;

        /// <summary>
        /// Game ratings
        /// </summary>
        [JsonProperty("ratings", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Ratings { get; set; } = null;

        /// <summary>
        /// Game score
        /// </summary>
        [JsonProperty("score", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Score { get; set; } = null;

        /// <summary>
        /// Is the machine enabled
        /// </summary>
        [JsonProperty("enabled", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Enabled { get; set; } = null; // bool?

        /// <summary>
        /// Does the game have a CRC check
        /// </summary>
        [JsonProperty("hascrc", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? HasCrc { get; set; } = null;

        /// <summary>
        /// Machine relations
        /// </summary>
        [JsonProperty("relatedto", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RelatedTo { get; set; } = null;

        #endregion

        #region OpenMSX Fields

        /// <summary>
        /// Generation MSX ID
        /// </summary>
        [JsonProperty("genmsxid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string GenMSXID { get; set; } = null;

        /// <summary>
        /// MSX System
        /// </summary>
        [JsonProperty("system", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string System { get; set; } = null;

        /// <summary>
        /// Machine country of origin
        /// </summary>
        [JsonProperty("country", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Country { get; set; } = null;

        #endregion

        #region SoftwareList Fields

        /// <summary>
        /// Support status
        /// </summary>
        [JsonProperty("supported", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Supported Supported { get; set; } = Supported.NULL;

        /// <summary>
        /// List of info items
        /// </summary>
        /// <remarks>Also in SoftwareList</remarks>
        [JsonProperty("infos", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SoftwareListInfo> Infos { get; set; } = null;

        /// <summary>
        /// List of shared feature items
        /// </summary>
        [JsonProperty("sharedfeat", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SoftwareListSharedFeature> SharedFeatures { get; set; } = null;

        #endregion

        #endregion

        #region Accessors

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public void SetFields(Dictionary<Field, string> mappings)
        {
            #region Common

            if (mappings.Keys.Contains(Field.Machine_Name))
                Name = mappings[Field.Machine_Name];

            if (mappings.Keys.Contains(Field.Machine_Comment))
                Comment = mappings[Field.Machine_Comment];

            if (mappings.Keys.Contains(Field.Machine_Description))
                Description = mappings[Field.Machine_Description];

            if (mappings.Keys.Contains(Field.Machine_Year))
                Year = mappings[Field.Machine_Year];

            if (mappings.Keys.Contains(Field.Machine_Manufacturer))
                Manufacturer = mappings[Field.Machine_Manufacturer];

            if (mappings.Keys.Contains(Field.Machine_Publisher))
                Publisher = mappings[Field.Machine_Publisher];

            if (mappings.Keys.Contains(Field.Machine_Category))
                Category = mappings[Field.Machine_Category];

            if (mappings.Keys.Contains(Field.Machine_RomOf))
                RomOf = mappings[Field.Machine_RomOf];

            if (mappings.Keys.Contains(Field.Machine_CloneOf))
                CloneOf = mappings[Field.Machine_CloneOf];

            if (mappings.Keys.Contains(Field.Machine_SampleOf))
                SampleOf = mappings[Field.Machine_SampleOf];

            if (mappings.Keys.Contains(Field.Machine_Type))
                MachineType = mappings[Field.Machine_Type].AsMachineType();

            #endregion

            #region AttractMode

            if (mappings.Keys.Contains(Field.Machine_Players))
                Players = mappings[Field.Machine_Players];

            if (mappings.Keys.Contains(Field.Machine_Rotation))
                Rotation = mappings[Field.Machine_Rotation];

            if (mappings.Keys.Contains(Field.Machine_Control))
                Control = mappings[Field.Machine_Control];

            if (mappings.Keys.Contains(Field.Machine_Status))
                Status = mappings[Field.Machine_Status];

            if (mappings.Keys.Contains(Field.Machine_DisplayCount))
                DisplayCount = mappings[Field.Machine_DisplayCount];

            if (mappings.Keys.Contains(Field.Machine_DisplayType))
                DisplayType = mappings[Field.Machine_DisplayType];

            if (mappings.Keys.Contains(Field.Machine_Buttons))
                Buttons = mappings[Field.Machine_Buttons];

            #endregion

            #region ListXML

            if (mappings.Keys.Contains(Field.Machine_SourceFile))
                SourceFile = mappings[Field.Machine_SourceFile];

            if (mappings.Keys.Contains(Field.Machine_Runnable))
                Runnable = mappings[Field.Machine_Runnable].AsRunnable();

            // TODO: Add Machine_DeviceReference*
            // TODO: Add Machine_Chip*
            // TODO: Add Machine_Display*
            // TODO: Add Machine_Sound*
            // TODO: Add Machine_Condition*
            // TODO: Add Machine_Input*
            // TODO: Add Machine_DipSwitch*
            // TODO: Add Machine_Configuration*
            // TODO: Add Machine_Port*
            // TODO: Add Machine_Adjuster*
            // TODO: Add Machine_Driver*
            // TODO: Add Machine_Feature*
            // TODO: Add Machine_Device*
            // TODO: Add Machine_Slot*
            // TODO: Add Machine_SoftwareList*
            // TODO: Add Machine_RamOption*

            #endregion

            #region Logiqx

            if (mappings.Keys.Contains(Field.Machine_Board))
                Board = mappings[Field.Machine_Board];

            if (mappings.Keys.Contains(Field.Machine_RebuildTo))
                RebuildTo = mappings[Field.Machine_RebuildTo];

            #endregion

            #region Logiqx EmuArc

            if (mappings.Keys.Contains(Field.Machine_TitleID))
                TitleID = mappings[Field.Machine_TitleID];

            if (mappings.Keys.Contains(Field.Machine_Developer))
                Developer = mappings[Field.Machine_Developer];

            if (mappings.Keys.Contains(Field.Machine_Genre))
                Genre = mappings[Field.Machine_Genre];

            if (mappings.Keys.Contains(Field.Machine_Subgenre))
                Subgenre = mappings[Field.Machine_Subgenre];

            if (mappings.Keys.Contains(Field.Machine_Ratings))
                Ratings = mappings[Field.Machine_Ratings];

            if (mappings.Keys.Contains(Field.Machine_Score))
                Score = mappings[Field.Machine_Score];

            if (mappings.Keys.Contains(Field.Machine_Enabled))
                Enabled = mappings[Field.Machine_Enabled];

            if (mappings.Keys.Contains(Field.Machine_CRC))
                HasCrc = mappings[Field.Machine_CRC].AsYesNo();

            if (mappings.Keys.Contains(Field.Machine_RelatedTo))
                RelatedTo = mappings[Field.Machine_RelatedTo];

            #endregion

            #region OpenMSX

            if (mappings.Keys.Contains(Field.Machine_GenMSXID))
                GenMSXID = mappings[Field.Machine_GenMSXID];

            if (mappings.Keys.Contains(Field.Machine_System))
                System = mappings[Field.Machine_System];

            if (mappings.Keys.Contains(Field.Machine_Country))
                Country = mappings[Field.Machine_Country];

            #endregion

            #region SoftwareList

            if (mappings.Keys.Contains(Field.Machine_Supported))
                Supported = mappings[Field.Machine_Supported].AsSupported();

            // TODO: Add Machine_Info*
            // TODO: Add Machine_SharedFeature*

            #endregion
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new Machine object
        /// </summary>
        public Machine()
        {
        }

        /// <summary>
        /// Create a new Machine object with the included information
        /// </summary>
        /// <param name="name">Name of the machine</param>
        /// <param name="description">Description of the machine</param>
        public Machine(string name, string description)
        {
            Name = name;
            Description = description;
        }

        #endregion

        #region Cloning methods

        /// <summary>
        /// Create a clone of the current machine
        /// </summary>
        /// <returns>New machine with the same values as the current one</returns>
        public object Clone()
        {
            return new Machine()
            {
                #region Common

                Name = this.Name,
                Comment = this.Comment,
                Description = this.Description,
                Year = this.Year,
                Manufacturer = this.Manufacturer,
                Publisher = this.Publisher,
                Category = this.Category,
                RomOf = this.RomOf,
                CloneOf = this.CloneOf,
                SampleOf = this.SampleOf,

                #endregion

                #region AttractMode

                Players = this.Players,
                Rotation = this.Rotation,
                Control = this.Control,
                Status = this.Status,
                DisplayCount = this.DisplayCount,
                DisplayType = this.DisplayType,
                Buttons = this.Buttons,

                #endregion

                #region ListXML

                SourceFile = this.SourceFile,
                Runnable = this.Runnable,
                DeviceReferences = this.DeviceReferences,
                Slots = this.Slots,
                Infos = this.Infos,
                MachineType = this.MachineType,

                #endregion

                #region Logiqx

                Board = this.Board,
                RebuildTo = this.RebuildTo,

                #endregion

                #region Logiqx EmuArc

                TitleID = this.TitleID,
                Developer = this.Developer,
                Genre = this.Genre,
                Subgenre = this.Subgenre,
                Ratings = this.Ratings,
                Score = this.Score,
                Enabled = this.Enabled,
                HasCrc = this.HasCrc,
                RelatedTo = this.RelatedTo,

                #endregion

                #region OpenMSX

                GenMSXID = this.GenMSXID,
                System = this.System,
                Country = this.Country,

                #endregion

                #region SoftwareList

                Supported = this.Supported,
                SharedFeatures = this.SharedFeatures,
                DipSwitches = this.DipSwitches,

                #endregion
            };
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Check to see if a Machine passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public bool PassesFilter(Filter filter)
        {
            #region Common

            // Machine_Name
            bool? machineNameFound = filter.Machine_Name.MatchesPositiveSet(Name);
            if (filter.IncludeOfInGame)
            {
                machineNameFound |= (filter.Machine_Name.MatchesPositiveSet(CloneOf) == true);
                machineNameFound |= (filter.Machine_Name.MatchesPositiveSet(RomOf) == true);
            }
            if (machineNameFound == false)
                return false;

            machineNameFound = filter.Machine_Name.MatchesNegativeSet(Name);
            if (filter.IncludeOfInGame)
            {
                machineNameFound |= (filter.Machine_Name.MatchesNegativeSet(CloneOf) == true);
                machineNameFound |= (filter.Machine_Name.MatchesNegativeSet(RomOf) == true);
            }
            if (machineNameFound == false)
                return false;

            // Machine_Comment
            if (filter.Machine_Comment.MatchesPositiveSet(Comment) == false)
                return false;
            if (filter.Machine_Comment.MatchesNegativeSet(Comment) == true)
                return false;

            // Machine_Description
            if (filter.Machine_Description.MatchesPositiveSet(Description) == false)
                return false;
            if (filter.Machine_Description.MatchesNegativeSet(Description) == true)
                return false;

            // Machine_Year
            if (filter.Machine_Year.MatchesPositiveSet(Year) == false)
                return false;
            if (filter.Machine_Year.MatchesNegativeSet(Year) == true)
                return false;

            // Machine_Manufacturer
            if (filter.Machine_Manufacturer.MatchesPositiveSet(Manufacturer) == false)
                return false;
            if (filter.Machine_Manufacturer.MatchesNegativeSet(Manufacturer) == true)
                return false;

            // Machine_Publisher
            if (filter.Machine_Publisher.MatchesPositiveSet(Publisher) == false)
                return false;
            if (filter.Machine_Publisher.MatchesNegativeSet(Publisher) == true)
                return false;

            // Machine_Category
            if (filter.Machine_Category.MatchesPositiveSet(Category) == false)
                return false;
            if (filter.Machine_Category.MatchesNegativeSet(Category) == true)
                return false;

            // Machine_RomOf
            if (filter.Machine_RomOf.MatchesPositiveSet(RomOf) == false)
                return false;
            if (filter.Machine_RomOf.MatchesNegativeSet(RomOf) == true)
                return false;

            // Machine_CloneOf
            if (filter.Machine_CloneOf.MatchesPositiveSet(CloneOf) == false)
                return false;
            if (filter.Machine_CloneOf.MatchesNegativeSet(CloneOf) == true)
                return false;

            // Machine_SampleOf
            if (filter.Machine_SampleOf.MatchesPositiveSet(SampleOf) == false)
                return false;
            if (filter.Machine_SampleOf.MatchesNegativeSet(SampleOf) == true)
                return false;

            #endregion

            #region AttractMode

            // Machine_Players
            if (filter.Machine_Players.MatchesPositiveSet(Players) == false)
                return false;
            if (filter.Machine_Players.MatchesNegativeSet(Players) == true)
                return false;

            // Machine_Rotation
            if (filter.Machine_Rotation.MatchesPositiveSet(Rotation) == false)
                return false;
            if (filter.Machine_Rotation.MatchesNegativeSet(Rotation) == true)
                return false;

            // Machine_Control
            if (filter.Machine_Control.MatchesPositiveSet(Control) == false)
                return false;
            if (filter.Machine_Control.MatchesNegativeSet(Control) == true)
                return false;

            // Machine_Status
            if (filter.Machine_Status.MatchesPositiveSet(Status) == false)
                return false;
            if (filter.Machine_Status.MatchesNegativeSet(Status) == true)
                return false;

            // Machine_DisplayCount
            if (filter.Machine_DisplayCount.MatchesPositiveSet(DisplayCount) == false)
                return false;
            if (filter.Machine_DisplayCount.MatchesNegativeSet(DisplayCount) == true)
                return false;

            // Machine_DisplayType
            if (filter.Machine_DisplayType.MatchesPositiveSet(DisplayType) == false)
                return false;
            if (filter.Machine_DisplayType.MatchesNegativeSet(DisplayType) == true)
                return false;

            // Machine_Buttons
            if (filter.Machine_Buttons.MatchesPositiveSet(Buttons) == false)
                return false;
            if (filter.Machine_Buttons.MatchesNegativeSet(Buttons) == true)
                return false;

            #endregion

            #region ListXML

            // Filter on source file
            if (filter.Machine_SourceFile.MatchesPositiveSet(SourceFile) == false)
                return false;
            if (filter.Machine_SourceFile.MatchesNegativeSet(SourceFile) == true)
                return false;

            // Filter on runnable
            if (filter.Machine_Runnable.MatchesPositive(Runnable.NULL, Runnable) == false)
                return false;
            if (filter.Machine_Runnable.MatchesNegative(Runnable.NULL, Runnable) == true)
                return false;

            // TODO: Add Slot filter

            // Filter on machine type
            if (filter.Machine_Type.MatchesPositive(MachineType.NULL, MachineType) == false)
                return false;
            if (filter.Machine_Type.MatchesNegative(MachineType.NULL, MachineType) == true)
                return false;

            #endregion

            #region Logiqx

            // Machine_Board
            if (filter.Machine_Board.MatchesPositiveSet(Board) == false)
                return false;
            if (filter.Machine_Board.MatchesNegativeSet(Board) == true)
                return false;

            // Machine_RebuildTo
            if (filter.Machine_RebuildTo.MatchesPositiveSet(RebuildTo) == false)
                return false;
            if (filter.Machine_RebuildTo.MatchesNegativeSet(RebuildTo) == true)
                return false;

            #endregion

            #region Logiqx EmuArc

            // Machine_TitleID
            if (filter.Machine_TitleID.MatchesPositiveSet(TitleID) == false)
                return false;
            if (filter.Machine_TitleID.MatchesNegativeSet(TitleID) == true)
                return false;

            // Machine_Developer
            if (filter.Machine_Developer.MatchesPositiveSet(Developer) == false)
                return false;
            if (filter.Machine_Developer.MatchesNegativeSet(Developer) == true)
                return false;

            // Machine_Genre
            if (filter.Machine_Genre.MatchesPositiveSet(Genre) == false)
                return false;
            if (filter.Machine_Genre.MatchesNegativeSet(Genre) == true)
                return false;

            // Machine_Subgenre
            if (filter.Machine_Subgenre.MatchesPositiveSet(Subgenre) == false)
                return false;
            if (filter.Machine_Subgenre.MatchesNegativeSet(Subgenre) == true)
                return false;

            // Machine_Ratings
            if (filter.Machine_Ratings.MatchesPositiveSet(Ratings) == false)
                return false;
            if (filter.Machine_Ratings.MatchesNegativeSet(Ratings) == true)
                return false;

            // Machine_Score
            if (filter.Machine_Score.MatchesPositiveSet(Score) == false)
                return false;
            if (filter.Machine_Score.MatchesNegativeSet(Score) == true)
                return false;

            // Machine_Enabled
            if (filter.Machine_Enabled.MatchesPositiveSet(Enabled) == false)
                return false;
            if (filter.Machine_Enabled.MatchesNegativeSet(Enabled) == true)
                return false;

            // Machine_CRC
            if (filter.Machine_CRC.MatchesNeutral(null, HasCrc) == false)
                return false;

            // Machine_RelatedTo
            if (filter.Machine_RelatedTo.MatchesPositiveSet(RelatedTo) == false)
                return false;
            if (filter.Machine_RelatedTo.MatchesNegativeSet(RelatedTo) == true)
                return false;

            #endregion

            #region OpenMSX

            // Machine_GenMSXID
            if (filter.Machine_GenMSXID.MatchesPositiveSet(GenMSXID) == false)
                return false;
            if (filter.Machine_GenMSXID.MatchesNegativeSet(GenMSXID) == true)
                return false;

            // Machine_System
            if (filter.Machine_System.MatchesPositiveSet(System) == false)
                return false;
            if (filter.Machine_System.MatchesNegativeSet(System) == true)
                return false;

            // Machine_Country
            if (filter.Machine_Country.MatchesPositiveSet(Country) == false)
                return false;
            if (filter.Machine_Country.MatchesNegativeSet(Country) == true)
                return false;

            #endregion

            #region SoftwareList

            // Machine_Supported
            if (filter.Machine_Supported.MatchesPositive(Supported.NULL, Supported) == false)
                return false;
            if (filter.Machine_Supported.MatchesNegative(Supported.NULL, Supported) == true)
                return false;

            // Infos

            // Machine_Infos
            if (filter.Machine_Infos.MatchesNeutral(null, Infos?.Any() ?? false) == false)
                return false;

            // Machine_Info_Name
            if (Infos.Any())
            {
                bool anyPositive = false;
                bool anyNegative = false;

                foreach (var info in Infos)
                {
                    if (filter.Machine_Info_Name.MatchesPositive(null, info?.Name) != false)
                        anyPositive = true;
                    if (filter.Machine_Info_Name.MatchesNegative(null, info?.Name) == true)
                        anyNegative = true;
                }

                if (!anyPositive)
                    return false;
                if (anyNegative)
                    return false;
            }

            // Machine_Info_Value
            if (Infos.Any())
            {
                bool anyPositive = false;
                bool anyNegative = false;

                foreach (var info in Infos)
                {
                    if (filter.Machine_Info_Value.MatchesPositive(null, info?.Value) != false)
                        anyPositive = true;
                    if (filter.Machine_Info_Value.MatchesNegative(null, info?.Value) == true)
                        anyNegative = true;
                }

                if (!anyPositive)
                    return false;
                if (anyNegative)
                    return false;
            }

            // SharedFeatures

            // Machine_SharedFeatures
            if (filter.Machine_SharedFeatures.MatchesNeutral(null, SharedFeatures?.Any() ?? false) == false)
                return false;

            // Machine_SharedFeature_Name
            if (SharedFeatures.Any())
            {
                bool anyPositive = false;
                bool anyNegative = false;

                foreach (var sharedFeature in SharedFeatures)
                {
                    if (filter.Machine_SharedFeature_Name.MatchesPositive(null, sharedFeature?.Name) != false)
                        anyPositive = true;
                    if (filter.Machine_SharedFeature_Name.MatchesNegative(null, sharedFeature?.Name) == true)
                        anyNegative = true;
                }

                if (!anyPositive)
                    return false;
                if (anyNegative)
                    return false;
            }

            // Machine_SharedFeature_Value
            if (SharedFeatures.Any())
            {
                bool anyPositive = false;
                bool anyNegative = false;

                foreach (var sharedFeature in SharedFeatures)
                {
                    if (filter.Machine_SharedFeature_Value.MatchesPositive(null, sharedFeature?.Value) != false)
                        anyPositive = true;
                    if (filter.Machine_SharedFeature_Value.MatchesNegative(null, sharedFeature?.Value) == true)
                        anyNegative = true;
                }

                if (!anyPositive)
                    return false;
                if (anyNegative)
                    return false;
            }

            #endregion

            return true;
        }

        /// <summary>
        /// Remove fields from the Machine
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        public void RemoveFields(List<Field> fields)
        {
            #region Common

            if (fields.Contains(Field.Machine_Name))
                Name = null;

            if (fields.Contains(Field.Machine_Comment))
                Comment = null;

            if (fields.Contains(Field.Machine_Description))
                Description = null;

            if (fields.Contains(Field.Machine_Year))
                Year = null;

            if (fields.Contains(Field.Machine_Manufacturer))
                Manufacturer = null;

            if (fields.Contains(Field.Machine_Publisher))
                Publisher = null;

            if (fields.Contains(Field.Machine_Category))
                Category = null;

            if (fields.Contains(Field.Machine_RomOf))
                RomOf = null;

            if (fields.Contains(Field.Machine_CloneOf))
                CloneOf = null;

            if (fields.Contains(Field.Machine_SampleOf))
                SampleOf = null;

            if (fields.Contains(Field.Machine_Type))
                MachineType = MachineType.NULL;

            #endregion

            #region AttractMode

            if (fields.Contains(Field.Machine_Players))
                Players = null;

            if (fields.Contains(Field.Machine_Rotation))
                Rotation = null;

            if (fields.Contains(Field.Machine_Control))
                Control = null;

            if (fields.Contains(Field.Machine_Status))
                Status = null;

            if (fields.Contains(Field.Machine_DisplayCount))
                DisplayCount = null;

            if (fields.Contains(Field.Machine_DisplayType))
                DisplayType = null;

            if (fields.Contains(Field.Machine_Buttons))
                Buttons = null;

            #endregion

            #region ListXML

            if (fields.Contains(Field.Machine_SourceFile))
                SourceFile = null;

            if (fields.Contains(Field.Machine_Runnable))
                Runnable = Runnable.NULL;

            if (fields.Contains(Field.Machine_DeviceReferences))
                DeviceReferences = null;

            if (fields.Contains(Field.Machine_Slots))
                Slots = null;

            if (fields.Contains(Field.Machine_Infos))
                Infos = null;

            #endregion

            #region Logiqx

            if (fields.Contains(Field.Machine_Board))
                Board = null;

            if (fields.Contains(Field.Machine_RebuildTo))
                RebuildTo = null;

            #endregion

            #region Logiqx EmuArc

            if (fields.Contains(Field.Machine_TitleID))
                TitleID = null;

            if (fields.Contains(Field.Machine_Developer))
                Developer = null;

            if (fields.Contains(Field.Machine_Genre))
                Genre = null;

            if (fields.Contains(Field.Machine_Subgenre))
                Subgenre = null;

            if (fields.Contains(Field.Machine_Ratings))
                Ratings = null;

            if (fields.Contains(Field.Machine_Score))
                Score = null;

            if (fields.Contains(Field.Machine_Enabled))
                Enabled = null;

            if (fields.Contains(Field.Machine_CRC))
                HasCrc = null;

            if (fields.Contains(Field.Machine_RelatedTo))
                RelatedTo = null;

            #endregion

            #region OpenMSX

            if (fields.Contains(Field.Machine_GenMSXID))
                GenMSXID = null;

            if (fields.Contains(Field.Machine_System))
                System = null;

            if (fields.Contains(Field.Machine_Country))
                Country = null;

            #endregion

            #region SoftwareList

            if (fields.Contains(Field.Machine_Supported))
                Supported = Supported.NULL;

            if (fields.Contains(Field.Machine_SharedFeatures))
                SharedFeatures = null;

            if (fields.Contains(Field.Machine_DipSwitches))
                DipSwitches = null;

            #endregion
        }

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Replace machine fields from another item
        /// </summary>
        /// <param name="item">DatItem to pull new information from</param>
        /// <param name="fields">List of Fields representing what should be updated</param>
        /// <param name="onlySame">True if descriptions should only be replaced if the game name is the same, false otherwise</param>
        public void ReplaceFields(Machine machine, List<Field> fields, bool onlySame)
        {
            #region Common

            if (fields.Contains(Field.Machine_Name))
                Name = machine.Name;

            if (fields.Contains(Field.Machine_Comment))
                Comment = machine.Comment;

            if (fields.Contains(Field.Machine_Description))
            {
                if (!onlySame || (onlySame && Name == Description))
                    Description = machine.Description;
            }

            if (fields.Contains(Field.Machine_Year))
                Year = machine.Year;

            if (fields.Contains(Field.Machine_Manufacturer))
                Manufacturer = machine.Manufacturer;

            if (fields.Contains(Field.Machine_Publisher))
                Publisher = machine.Publisher;

            if (fields.Contains(Field.Machine_Category))
                Category = machine.Category;

            if (fields.Contains(Field.Machine_RomOf))
                RomOf = machine.RomOf;

            if (fields.Contains(Field.Machine_CloneOf))
                CloneOf = machine.CloneOf;

            if (fields.Contains(Field.Machine_SampleOf))
                SampleOf = machine.SampleOf;

            if (fields.Contains(Field.Machine_Type))
                MachineType = machine.MachineType;

            #endregion

            #region AttractMode

            if (fields.Contains(Field.Machine_Players))
                Players = machine.Players;

            if (fields.Contains(Field.Machine_Rotation))
                Rotation = machine.Rotation;

            if (fields.Contains(Field.Machine_Control))
                Control = machine.Control;

            if (fields.Contains(Field.Machine_Status))
                Status = machine.Status;

            if (fields.Contains(Field.Machine_DisplayCount))
                DisplayCount = machine.DisplayCount;

            if (fields.Contains(Field.Machine_DisplayType))
                DisplayType = machine.DisplayType;

            if (fields.Contains(Field.Machine_Buttons))
                Buttons = machine.Buttons;

            #endregion

            #region ListXML

            if (fields.Contains(Field.Machine_SourceFile))
                SourceFile = machine.SourceFile;

            if (fields.Contains(Field.Machine_Runnable))
                Runnable = machine.Runnable;

            if (fields.Contains(Field.Machine_DeviceReferences))
                DeviceReferences = machine.DeviceReferences;

            if (fields.Contains(Field.Machine_Slots))
                Slots = machine.Slots;

            if (fields.Contains(Field.Machine_Infos))
                Infos = machine.Infos;

            #endregion

            #region Logiqx

            if (fields.Contains(Field.Machine_Board))
                Board = machine.Board;

            if (fields.Contains(Field.Machine_RebuildTo))
                RebuildTo = machine.RebuildTo;

            #endregion

            #region Logiqx EmuArc

            if (fields.Contains(Field.Machine_TitleID))
                TitleID = machine.TitleID;

            if (fields.Contains(Field.Machine_Developer))
                Developer = machine.Developer;

            if (fields.Contains(Field.Machine_Genre))
                Genre = machine.Genre;

            if (fields.Contains(Field.Machine_Subgenre))
                Subgenre = machine.Subgenre;

            if (fields.Contains(Field.Machine_Ratings))
                Ratings = machine.Ratings;

            if (fields.Contains(Field.Machine_Score))
                Score = machine.Score;

            if (fields.Contains(Field.Machine_Enabled))
                Enabled = machine.Enabled;

            if (fields.Contains(Field.Machine_CRC))
                HasCrc = machine.HasCrc;

            if (fields.Contains(Field.Machine_RelatedTo))
                RelatedTo = machine.RelatedTo;

            #endregion

            #region OpenMSX

            if (fields.Contains(Field.Machine_GenMSXID))
                GenMSXID = machine.GenMSXID;

            if (fields.Contains(Field.Machine_System))
                System = machine.System;

            if (fields.Contains(Field.Machine_Country))
                Country = machine.Country;

            #endregion

            #region SoftwareList

            if (fields.Contains(Field.Machine_Supported))
                Supported = machine.Supported;

            if (fields.Contains(Field.Machine_SharedFeatures))
                SharedFeatures = machine.SharedFeatures;

            if (fields.Contains(Field.Machine_DipSwitches))
                DipSwitches = machine.DipSwitches;

            #endregion
        }

        #endregion
    }
}
