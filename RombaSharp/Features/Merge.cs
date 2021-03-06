﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

using SabreTools.Library.Help;
using SabreTools.Library.IO;

namespace RombaSharp.Features
{
    internal class Merge : BaseFeature
    {
        public const string Value = "Merge";

        public Merge()
        {
            Name = Value;
            Flags = new List<string>() { "merge" };
            Description = "Merges depot";
            _featureType = FeatureType.Flag;
            LongDescription = "Merges specified depot into current depot.";
            Features = new Dictionary<string, Feature>();

            AddFeature(OnlyNeededFlag);
            AddFeature(ResumeStringInput);
            AddFeature(WorkersInt32Input);
            AddFeature(SkipInitialScanFlag);
        }

        // TODO: Add way of specifying "current depot" since that's what Romba relies on
        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);

            // Get feature flags
            bool onlyNeeded = GetBoolean(features, OnlyNeededValue);
            bool skipInitialscan = GetBoolean(features, SkipInitialScanValue);
            int workers = GetInt32(features, WorkersInt32Value);
            string resume = GetString(features, ResumeStringValue);

            logger.Error("This feature is not yet implemented: merge");

            // Verify that the inputs are valid directories
            Inputs = DirectoryExtensions.GetDirectoriesOnly(Inputs).Select(p => p.CurrentPath).ToList();

            // Loop over all input directories
            foreach (string input in Inputs)
            {
                List<string> depotFiles = Directory.EnumerateFiles(input, "*.gz", SearchOption.AllDirectories).ToList();

                // If we are copying all that is possible but we want to scan first
                if (!onlyNeeded && !skipInitialscan)
                {

                }

                // If we are copying all that is possible but we don't care to scan first
                else if (!onlyNeeded && skipInitialscan)
                {

                }

                // If we are copying only what is needed but we want to scan first
                else if (onlyNeeded && !skipInitialscan)
                {

                }

                // If we are copying only what is needed but we don't care to scan first
                else if (onlyNeeded && skipInitialscan)
                {

                }
            }
        }
    }
}
