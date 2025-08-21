namespace MTGPlexer.TokenAnalysis.RegexDTOs
{
    public record ValueCaptureVariantSet
    {
        public string CanonicalRepresentation { get; }
        public int TotalCount { get; }
        public Dictionary<RegexCapturePosition, int> VariantCounts { get; } = [];

        public ValueCaptureVariantSet(ValueCaptureVariantCollector collector, string regexUsedForCapture, string propCaptureGroupName)
        {
            CanonicalRepresentation = collector.CanonicalRepresentation;
            TotalCount = collector.VariantCounts.Sum(x => x.Value);
            var sortedCounts = collector.VariantCounts.OrderByDescending(x => x.Value);
            PopulateVariantCounts(sortedCounts, regexUsedForCapture, propCaptureGroupName);
        }

        /// <summary>
        /// Populates the variant counts by building a fast lookup map of potential captures to their regex positions.
        /// This approach is optimized for the common case of simple alternatives (e.g., a|b|c).
        /// </summary>
        private void PopulateVariantCounts(IOrderedEnumerable<KeyValuePair<string, int>> sortedCounts, string regexUsedForCapture, string propCaptureGroupName)
        {
            try
            {
                var groupPattern = $@"\(\?<{propCaptureGroupName}>(?<content>.*)\)";
                var groupMatch = Regex.Match(regexUsedForCapture, groupPattern);

                if (groupMatch.Success)
                {
                    var groupContent = groupMatch.Groups["content"].Value;
                    var groupStartIndex = groupMatch.Groups["content"].Index;

                    // 1. Build the fast lookup dictionary ONE TIME.
                    var positionLookup = BuildVariantPositionLookup(groupContent, groupStartIndex);

                    // 2. Use the fast lookup for each variant.
                    foreach (var variant in sortedCounts)
                    {
                        if (positionLookup.TryGetValue(variant.Key, out var position))
                        {
                            VariantCounts.Add(position, variant.Value);
                        }
                        else
                        {
                            // Fallback for variants not found in the lookup.
                            var notFoundPosition = new RegexCapturePosition(variant.Key, -1, -1);
                            VariantCounts.Add(notFoundPosition, variant.Value);
                        }
                    }
                }
                else
                {
                    AddAllAsNotFound(sortedCounts);
                }
            }
            catch (Exception)
            {
                AddAllAsNotFound(sortedCounts);
            }
        }

        /// <summary>
        /// Parses the content of a regex capture group and builds a dictionary that maps
        /// each possible string it can match to its exact position in the parent regex.
        /// </summary>
        /// <param name="groupContent">The inner content of the capture group (e.g., "discard(s)?|draw(s)?").</param>
        /// <param name="groupStartIndex">The starting index of the group content within the full regex string.</param>
        /// <returns>A dictionary mapping variant strings to their capture positions.</returns>
        private Dictionary<string, RegexCapturePosition> BuildVariantPositionLookup(string groupContent, int groupStartIndex)
        {
            var lookup = new Dictionary<string, RegexCapturePosition>();
            var alternatives = groupContent.Split('|');

            foreach (var alternative in alternatives)
            {
                // Find the position of this specific alternative within the original regex string.
                int alternativeStartIndexInGroup = groupContent.IndexOf(alternative, StringComparison.Ordinal);
                int startPosition = groupStartIndex + alternativeStartIndexInGroup;
                int endPosition = startPosition + alternative.Length;

                var regexCapturePosition = new RegexCapturePosition(alternative, startPosition, endPosition);

                // Handle common, simple regex syntax without using the Regex engine.
                // This is the core of the performance improvement.
                if (alternative.EndsWith("(s)?"))
                {
                    string baseWord = alternative.Substring(0, alternative.Length - 4);
                    lookup[baseWord] = regexCapturePosition;
                    lookup[baseWord + "s"] = regexCapturePosition;
                }
                else if (alternative.EndsWith("s?"))
                {
                    string baseWord = alternative.Substring(0, alternative.Length - 2);
                    lookup[baseWord] = regexCapturePosition;
                    lookup[baseWord + "s"] = regexCapturePosition;
                }
                else
                {
                    // It's a plain string literal.
                    lookup[alternative] = regexCapturePosition;
                }
            }

            return lookup;
        }

        /// <summary>
        /// Helper method to populate the variant counts with a "not found" position.
        /// </summary>
        private void AddAllAsNotFound(IOrderedEnumerable<KeyValuePair<string, int>> sortedCounts)
        {
            foreach (var variant in sortedCounts)
            {
                var regexCapturePosition = new RegexCapturePosition(variant.Key, -1, -1);
                VariantCounts.Add(regexCapturePosition, variant.Value);
            }
        }

        public override string ToString()
        {
            var mainString = $"{CanonicalRepresentation}: {TotalCount}";

            if (!VariantCounts.Select(x => x.Key).Any(x => x.Capture != CanonicalRepresentation))
                return mainString;

            var subPartStr = string.Join(" | ", VariantCounts.Select(x => $"{x.Key}: {x.Value}"));

            return $"{mainString} ({subPartStr})";
        }
    }
}