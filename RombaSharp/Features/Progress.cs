﻿using System.Collections.Generic;

using SabreTools.Library.Help;

namespace RombaSharp.Features
{
    internal class Progress : BaseFeature
    {
        public const string Value = "Progress";

        public Progress()
        {
            Name = Value;
            Flags = new List<string>() { "progress" };
            Description = "Shows progress of the currently running command.";
            _featureType = FeatureType.Flag;
            LongDescription = "Shows progress of the currently running command.";
            Features = new Dictionary<string, Feature>();
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);
            logger.User("This feature is not yet implemented: progress");
        }
    }
}
