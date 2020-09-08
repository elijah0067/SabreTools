﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents one machine display
    /// </summary>
    [JsonObject("display")]
    [XmlRoot("display")]
    public class Display : DatItem
    {
        #region Fields

        /// <summary>
        /// Display tag
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Display type
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("type")]
        public DisplayType DisplayType { get; set; }

        [JsonIgnore]
        public bool DisplayTypeSpecified { get { return DisplayType != DisplayType.NULL; } }

        /// <summary>
        /// Display rotation
        /// </summary>
        [JsonProperty("rotate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("rotate")]
        public long? Rotate { get; set; }

        [JsonIgnore]
        public bool RotateSpecified { get { return Rotate != null; } }

        /// <summary>
        /// Determines if display is flipped in the X-coordinates
        /// </summary>
        [JsonProperty("flipx", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("flipx")]
        public bool? FlipX { get; set; }

        [JsonIgnore]
        public bool FlipXSpecified { get { return FlipX != null; } }

        /// <summary>
        /// Display width
        /// </summary>
        [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("width")]
        public long? Width { get; set; }

        [JsonIgnore]
        public bool WidthSpecified { get { return Width != null; } }

        /// <summary>
        /// Display height
        /// </summary>
        [JsonProperty("height", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("height")]
        public long? Height { get; set; }

        [JsonIgnore]
        public bool HeightSpecified { get { return Height != null; } }

        /// <summary>
        /// Refresh rate
        /// </summary>
        [JsonProperty("refresh", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("refresh")]
        public double? Refresh { get; set; }

        [JsonIgnore]
        public bool RefreshSpecified { get { return Refresh != null; } }

        /// <summary>
        /// Pixel clock timer
        /// </summary>
        [JsonProperty("pixclock", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("pixclock")]
        public long? PixClock { get; set; }

        [JsonIgnore]
        public bool PixClockSpecified { get { return PixClock != null; } }

        /// <summary>
        /// Total horizontal lines
        /// </summary>
        [JsonProperty("htotal", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("htotal")]
        public long? HTotal { get; set; }

        [JsonIgnore]
        public bool HTotalSpecified { get { return HTotal != null; } }

        /// <summary>
        /// Horizontal blank end
        /// </summary>
        [JsonProperty("hbend", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("hbend")]
        public long? HBEnd { get; set; }

        [JsonIgnore]
        public bool HBEndSpecified { get { return HBEnd != null; } }

        /// <summary>
        /// Horizontal blank start
        /// </summary>
        [JsonProperty("hbstart", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("hbstart")]
        public long? HBStart { get; set; }

        [JsonIgnore]
        public bool HBStartSpecified { get { return HBStart != null; } }

        /// <summary>
        /// Total vertical lines
        /// </summary>
        [JsonProperty("vtotal", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("vtotal")]
        public long? VTotal { get; set; }

        [JsonIgnore]
        public bool VTotalSpecified { get { return VTotal != null; } }

        /// <summary>
        /// Vertical blank end
        /// </summary>
        [JsonProperty("vbend", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("vbend")]
        public long? VBEnd { get; set; }

        [JsonIgnore]
        public bool VBEndSpecified { get { return VBEnd != null; } }

        /// <summary>
        /// Vertical blank start
        /// </summary>
        [JsonProperty("vbstart", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("vbstart")]
        public long? VBStart { get; set; }

        [JsonIgnore]
        public bool VBStartSpecified { get { return VBStart != null; } }

        #endregion

        #region Accessors

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle Display-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Tag))
                Tag = mappings[Field.DatItem_Tag];

            if (mappings.Keys.Contains(Field.DatItem_DisplayType))
                DisplayType = mappings[Field.DatItem_DisplayType].AsDisplayType();

            if (mappings.Keys.Contains(Field.DatItem_Rotate))
                Rotate = Sanitizer.CleanLong(mappings[Field.DatItem_Rotate]);

            if (mappings.Keys.Contains(Field.DatItem_FlipX))
                FlipX = mappings[Field.DatItem_FlipX].AsYesNo();

            if (mappings.Keys.Contains(Field.DatItem_Width))
                Width = Sanitizer.CleanLong(mappings[Field.DatItem_Width]);

            if (mappings.Keys.Contains(Field.DatItem_Height))
                Height = Sanitizer.CleanLong(mappings[Field.DatItem_Height]);

            if (mappings.Keys.Contains(Field.DatItem_Refresh))
            {
                if (Double.TryParse(mappings[Field.DatItem_Refresh], out double refresh))
                    Refresh = refresh;
            }

            if (mappings.Keys.Contains(Field.DatItem_PixClock))
                PixClock = Sanitizer.CleanLong(mappings[Field.DatItem_PixClock]);

            if (mappings.Keys.Contains(Field.DatItem_HTotal))
                HTotal = Sanitizer.CleanLong(mappings[Field.DatItem_HTotal]);

            if (mappings.Keys.Contains(Field.DatItem_HBEnd))
                HBEnd = Sanitizer.CleanLong(mappings[Field.DatItem_HBEnd]);

            if (mappings.Keys.Contains(Field.DatItem_HBStart))
                HBStart = Sanitizer.CleanLong(mappings[Field.DatItem_HBStart]);

            if (mappings.Keys.Contains(Field.DatItem_VTotal))
                VTotal = Sanitizer.CleanLong(mappings[Field.DatItem_VTotal]);

            if (mappings.Keys.Contains(Field.DatItem_VBEnd))
                VBEnd = Sanitizer.CleanLong(mappings[Field.DatItem_VBEnd]);

            if (mappings.Keys.Contains(Field.DatItem_VBStart))
                VBStart = Sanitizer.CleanLong(mappings[Field.DatItem_VBStart]);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Display object
        /// </summary>
        public Display()
        {
            ItemType = ItemType.Display;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Display()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Tag = this.Tag,
                DisplayType = this.DisplayType,
                Rotate = this.Rotate,
                FlipX = this.FlipX,
                Width = this.Width,
                Height = this.Height,
                Refresh = this.Refresh,
                PixClock = this.PixClock,
                HTotal = this.HTotal,
                HBEnd = this.HBEnd,
                HBStart = this.HBStart,
                VTotal = this.VTotal,
                VBEnd = this.VBEnd,
                VBStart = this.VBStart,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Display, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Display
            Display newOther = other as Display;

            // If the Display information matches
            return (Tag == newOther.Tag
                && DisplayType == newOther.DisplayType
                && Rotate == newOther.Rotate
                && FlipX == newOther.FlipX
                && Width == newOther.Width
                && Height == newOther.Height
                && Refresh == newOther.Refresh
                && PixClock == newOther.PixClock
                && HTotal == newOther.HTotal
                && HBEnd == newOther.HBEnd
                && HBStart == newOther.HBStart
                && VTotal == newOther.VTotal
                && VBEnd == newOther.VBEnd
                && VBStart == newOther.VBStart);
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Check to see if a DatItem passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public override bool PassesFilter(Filter filter)
        {
            // Check common fields first
            if (!base.PassesFilter(filter))
                return false;

            // Filter on tag
            if (filter.DatItem_Tag.MatchesPositiveSet(Tag) == false)
                return false;
            if (filter.DatItem_Tag.MatchesNegativeSet(Tag) == true)
                return false;

            // Filter on display type
            if (filter.DatItem_DisplayType.MatchesPositive(DisplayType.NULL, DisplayType) == false)
                return false;
            if (filter.DatItem_DisplayType.MatchesNegative(DisplayType.NULL, DisplayType) == true)
                return false;

            // Filter on rotation
            if (filter.DatItem_Rotate.MatchesNeutral(null, Rotate) == false)
                return false;
            else if (filter.DatItem_Rotate.MatchesPositive(null, Rotate) == false)
                return false;
            else if (filter.DatItem_Rotate.MatchesNegative(null, Rotate) == false)
                return false;

            // Filter on flipx
            if (filter.DatItem_FlipX.MatchesNeutral(null, FlipX) == false)
                return false;

            // Filter on width
            if (filter.DatItem_Width.MatchesNeutral(null, Width) == false)
                return false;
            else if (filter.DatItem_Width.MatchesPositive(null, Width) == false)
                return false;
            else if (filter.DatItem_Width.MatchesNegative(null, Width) == false)
                return false;

            // Filter on height
            if (filter.DatItem_Height.MatchesNeutral(null, Height) == false)
                return false;
            else if (filter.DatItem_Height.MatchesPositive(null, Height) == false)
                return false;
            else if (filter.DatItem_Height.MatchesNegative(null, Height) == false)
                return false;

            // Filter on refresh
            if (filter.DatItem_Refresh.MatchesNeutral(null, Refresh) == false)
                return false;
            else if (filter.DatItem_Refresh.MatchesPositive(null, Refresh) == false)
                return false;
            else if (filter.DatItem_Refresh.MatchesNegative(null, Refresh) == false)
                return false;

            // Filter on pixclock
            if (filter.DatItem_PixClock.MatchesNeutral(null, PixClock) == false)
                return false;
            else if (filter.DatItem_PixClock.MatchesPositive(null, PixClock) == false)
                return false;
            else if (filter.DatItem_PixClock.MatchesNegative(null, PixClock) == false)
                return false;

            // Filter on htotal
            if (filter.DatItem_HTotal.MatchesNeutral(null, HTotal) == false)
                return false;
            else if (filter.DatItem_HTotal.MatchesPositive(null, HTotal) == false)
                return false;
            else if (filter.DatItem_HTotal.MatchesNegative(null, HTotal) == false)
                return false;

            // Filter on hbend
            if (filter.DatItem_HBEnd.MatchesNeutral(null, HBEnd) == false)
                return false;
            else if (filter.DatItem_HBEnd.MatchesPositive(null, HBEnd) == false)
                return false;
            else if (filter.DatItem_HBEnd.MatchesNegative(null, HBEnd) == false)
                return false;

            // Filter on hbstart
            if (filter.DatItem_HBStart.MatchesNeutral(null, HBStart) == false)
                return false;
            else if (filter.DatItem_HBStart.MatchesPositive(null, HBStart) == false)
                return false;
            else if (filter.DatItem_HBStart.MatchesNegative(null, HBStart) == false)
                return false;

            // Filter on vtotal
            if (filter.DatItem_VTotal.MatchesNeutral(null, VTotal) == false)
                return false;
            else if (filter.DatItem_VTotal.MatchesPositive(null, VTotal) == false)
                return false;
            else if (filter.DatItem_VTotal.MatchesNegative(null, VTotal) == false)
                return false;

            // Filter on vbend
            if (filter.DatItem_VBEnd.MatchesNeutral(null, VBEnd) == false)
                return false;
            else if (filter.DatItem_VBEnd.MatchesPositive(null, VBEnd) == false)
                return false;
            else if (filter.DatItem_VBEnd.MatchesNegative(null, VBEnd) == false)
                return false;

            // Filter on vbstart
            if (filter.DatItem_VBStart.MatchesNeutral(null, VBStart) == false)
                return false;
            else if (filter.DatItem_VBStart.MatchesPositive(null, VBStart) == false)
                return false;
            else if (filter.DatItem_VBStart.MatchesNegative(null, VBStart) == false)
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
            if (fields.Contains(Field.DatItem_Tag))
                Tag = null;

            if (fields.Contains(Field.DatItem_DisplayType))
                DisplayType = DisplayType.NULL;

            if (fields.Contains(Field.DatItem_Rotate))
                Rotate = null;

            if (fields.Contains(Field.DatItem_FlipX))
                FlipX = null;

            if (fields.Contains(Field.DatItem_Width))
                Width = null;

            if (fields.Contains(Field.DatItem_Height))
                Height = null;

            if (fields.Contains(Field.DatItem_Refresh))
                Refresh = null;

            if (fields.Contains(Field.DatItem_PixClock))
                PixClock = null;

            if (fields.Contains(Field.DatItem_HTotal))
                HTotal = null;

            if (fields.Contains(Field.DatItem_HBEnd))
                HBEnd = null;

            if (fields.Contains(Field.DatItem_HBStart))
                HBStart = null;

            if (fields.Contains(Field.DatItem_VTotal))
                VTotal = null;

            if (fields.Contains(Field.DatItem_VBEnd))
                VBEnd = null;

            if (fields.Contains(Field.DatItem_VBStart))
                VBStart = null;
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

            // If we don't have a Display to replace from, ignore specific fields
            if (item.ItemType != ItemType.Display)
                return;

            // Cast for easier access
            Display newItem = item as Display;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Tag))
                Tag = newItem.Tag;

            if (fields.Contains(Field.DatItem_DisplayType))
                DisplayType = newItem.DisplayType;

            if (fields.Contains(Field.DatItem_Rotate))
                Rotate = newItem.Rotate;

            if (fields.Contains(Field.DatItem_FlipX))
                FlipX = newItem.FlipX;

            if (fields.Contains(Field.DatItem_Width))
                Width = newItem.Width;

            if (fields.Contains(Field.DatItem_Height))
                Height = newItem.Height;

            if (fields.Contains(Field.DatItem_Refresh))
                Refresh = newItem.Refresh;

            if (fields.Contains(Field.DatItem_PixClock))
                PixClock = newItem.PixClock;

            if (fields.Contains(Field.DatItem_HTotal))
                HTotal = newItem.HTotal;

            if (fields.Contains(Field.DatItem_HBEnd))
                HBEnd = newItem.HBEnd;

            if (fields.Contains(Field.DatItem_HBStart))
                HBStart = newItem.HBStart;

            if (fields.Contains(Field.DatItem_VTotal))
                VTotal = newItem.VTotal;

            if (fields.Contains(Field.DatItem_VBEnd))
                VBEnd = newItem.VBEnd;

            if (fields.Contains(Field.DatItem_VBStart))
                VBStart = newItem.VBStart;
        }

        #endregion
    }
}
