﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents a single instance of another item
    /// </summary>
    [JsonObject("instance"), XmlRoot("instance")]
    public class Instance : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Short name for the instance
        /// </summary>
        [JsonProperty("briefname", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("briefname")]
        public string BriefName { get; set; }

        #endregion

        #region Accessors

        /// <summary>
        /// Gets the name to use for a DatItem
        /// </summary>
        /// <returns>Name if available, null otherwise</returns>
        public override string GetName()
        {
            return Name;
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle Instance-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Instance_Name))
                Name = mappings[Field.DatItem_Instance_Name];

            if (mappings.Keys.Contains(Field.DatItem_Instance_BriefName))
                BriefName = mappings[Field.DatItem_Instance_BriefName];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Instance object
        /// </summary>
        public Instance()
        {
            Name = string.Empty;
            ItemType = ItemType.Instance;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Instance()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                BriefName = this.BriefName,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Instance, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Instance
            Instance newOther = other as Instance;

            // If the Instance information matches
            return (Name == newOther.Name && BriefName == newOther.BriefName);
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Clean a DatItem according to the cleaner
        /// </summary>
        /// <param name="cleaner">Cleaner to implement</param>
        public override void Clean(Cleaner cleaner)
        {
            // Clean common items first
            base.Clean(cleaner);

            // If we're stripping unicode characters, strip item name
            if (cleaner?.RemoveUnicode == true)
                Name = Sanitizer.RemoveUnicodeCharacters(Name);

            // If we are in NTFS trim mode, trim the game name
            if (cleaner?.Trim == true)
            {
                // Windows max name length is 260
                int usableLength = 260 - Machine.Name.Length - (cleaner.Root?.Length ?? 0);
                if (Name.Length > usableLength)
                {
                    string ext = Path.GetExtension(Name);
                    Name = Name.Substring(0, usableLength - ext.Length);
                    Name += ext;
                }
            }
        }

        /// <summary>
        /// Check to see if a DatItem passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <param name="sub">True if this is a subitem, false otherwise</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public override bool PassesFilter(Filter filter, bool sub = false)
        {
            // Check common fields first
            if (!base.PassesFilter(filter, sub))
                return false;

            // Filter on item name
            if (!filter.PassStringFilter(filter.DatItem_Instance_Name, Name))
                return false;

            // Filter on brief name
            if (!filter.PassStringFilter(filter.DatItem_Instance_BriefName, BriefName))
                return false;

            return true;
        }

        /// <summary>
        /// Remove fields from the DatItem
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        public override void RemoveFields(List<Field> fields)
        {
            // Remove common fields first
            base.RemoveFields(fields);

            // Remove the fields
            if (fields.Contains(Field.DatItem_Instance_Name))
                Name = null;

            if (fields.Contains(Field.DatItem_Instance_BriefName))
                BriefName = null;
        }

        /// <summary>
        /// Set internal names to match One Rom Per Game (ORPG) logic
        /// </summary>
        public override void SetOneRomPerGame()
        {
            string[] splitname = Name.Split('.');
            Machine.Name += $"/{string.Join(".", splitname.Take(splitname.Length > 1 ? splitname.Length - 1 : 1))}";
            Name = Path.GetFileName(Name);
        }

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Replace fields from another item
        /// </summary>
        /// <param name="item">DatItem to pull new information from</param>
        /// <param name="fields">List of Fields representing what should be updated</param>
        public override void ReplaceFields(DatItem item, List<Field> fields)
        {
            // Replace common fields first
            base.ReplaceFields(item, fields);

            // If we don't have a Instance to replace from, ignore specific fields
            if (item.ItemType != ItemType.Instance)
                return;

            // Cast for easier access
            Instance newItem = item as Instance;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Instance_Name))
                Name = newItem.Name;

            if (fields.Contains(Field.DatItem_Instance_BriefName))
                BriefName = newItem.BriefName;
        }

        #endregion
    }
}
