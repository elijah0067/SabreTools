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
    /// Represents one ListXML confsetting or dipvalue
    /// </summary>
    [JsonObject("setting"), XmlRoot("setting")]
    public class Setting : DatItem
    {
        #region Fields

        /// <summary>
        /// Setting name
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Setting value
        /// </summary>
        [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("value")]
        public string Value { get; set; }

        /// <summary>
        /// Determines if the setting is default or not
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("default")]
        public bool? Default { get; set; }

        [JsonIgnore]
        public bool DefaultSpecified { get { return Default != null; } }

        /// <summary>
        /// List of conditions on the setting
        /// </summary>
        [JsonProperty("conditions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("conditions")]
        public List<Condition> Conditions { get; set; }

        [JsonIgnore]
        public bool ConditionsSpecified { get { return Conditions != null && Conditions.Count > 0; } }

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

            // Handle Setting-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Setting_Name))
                Name = mappings[Field.DatItem_Setting_Name];

            if (mappings.Keys.Contains(Field.DatItem_Setting_Value))
                Value = mappings[Field.DatItem_Setting_Value];

            if (mappings.Keys.Contains(Field.DatItem_Setting_Default))
                Default = mappings[Field.DatItem_Setting_Default].AsYesNo();

            if (ConditionsSpecified)
            {
                foreach (Condition condition in Conditions)
                {
                    condition.SetFields(mappings, true);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Setting object
        /// </summary>
        public Setting()
        {
            Name = string.Empty;
            ItemType = ItemType.Setting;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Setting()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                Value = this.Value,
                Default = this.Default,
                Conditions = this.Conditions,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Setting, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Setting
            Setting newOther = other as Setting;

            // If the Setting information matches
            bool match = (Name == newOther.Name
                && Value == newOther.Value
                && Default == newOther.Default);
            if (!match)
                return match;

            // If the conditions match
            if (ConditionsSpecified)
            {
                foreach (Condition condition in Conditions)
                {
                    match &= newOther.Conditions.Contains(condition);
                }
            }

            return match;
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
            if (!filter.PassStringFilter(filter.DatItem_Setting_Name, Name))
                return false;

            // Filter on value
            if (!filter.PassStringFilter(filter.DatItem_Setting_Value, Value))
                return false;

            // Filter on default
            if (!filter.PassBoolFilter(filter.DatItem_Setting_Default, Default))
                return false;

            // Filter on individual conditions
            if (ConditionsSpecified)
            {
                foreach (Condition condition in Conditions)
                {
                    if (!condition.PassesFilter(filter, true))
                        return false;
                }
            }

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
            if (fields.Contains(Field.DatItem_Setting_Name))
                Name = null;

            if (fields.Contains(Field.DatItem_Setting_Value))
                Value = null;

            if (fields.Contains(Field.DatItem_Setting_Default))
                Default = null;

            if (ConditionsSpecified)
            {
                foreach (Condition condition in Conditions)
                {
                    condition.RemoveFields(fields, true);
                }
            }
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

            // If we don't have a Setting to replace from, ignore specific fields
            if (item.ItemType != ItemType.Setting)
                return;

            // Cast for easier access
            Setting newItem = item as Setting;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Setting_Name))
                Name = newItem.Name;

            if (fields.Contains(Field.DatItem_Setting_Value))
                Value = newItem.Value;

            if (fields.Contains(Field.DatItem_Setting_Default))
                Default = newItem.Default;

            // DatItem_Condition_* doesn't make sense here
            // since not every condition under the other item
            // can replace every condition under this item
        }

        #endregion
    }
}
