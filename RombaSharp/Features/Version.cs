﻿using System.Collections.Generic;

using SabreTools.Library.Data;
using SabreTools.Library.Help;

namespace RombaSharp.Features
{
    internal class Version : BaseFeature
    {
        public const string Value = "Version";

        public Version()
        {
            Name = Value;
            Flags = new List<string>() { "version" };
            Description = "Prints version";
            _featureType = FeatureType.Flag;
            LongDescription = "Prints version.";
            Features = new Dictionary<string, Feature>();
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);
            logger.User($"RombaSharp version: {Constants.Version}");
        }
    }
}
