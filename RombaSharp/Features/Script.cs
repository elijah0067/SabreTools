﻿using System.Collections.Generic;

using SabreTools.Library.Help;

namespace RombaSharp.Features
{
    internal class Script : BaseFeature
    {
        public const string Value = "Script";

        public Script()
        {
            Name = Value;
            Flags = new List<string>() { "--script" };
            Description = "Enable script mode (no clear screen)";
            _featureType = FeatureType.Flag;
            LongDescription = "For times when RombaSharp is being used in a scripted environment, the user may not want the screen to be cleared every time that it is called. This flag allows the user to skip clearing the screen on run just like if the console was being redirected.";
            Features = new Dictionary<string, Feature>();
        }
    }
}
