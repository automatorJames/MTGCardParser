namespace MTGPlexer.TokenAnalysis.RegexDTOs
{
    public record ValueCaptureVariantSet
    {
        public string CanonicalRepresentation { get; }
        public string TerminalPropCaptureGroupName { get; }
        public int TotalCount { get; }
        public Dictionary<string, int> VariantCounts { get; } = [];

        public int CapturedByPrettyRegexLine { get; private set; } = -1;

        public ValueCaptureVariantSet(ValueCaptureVariantCollector collector, string terminalPropCaptureGroupName)
        {
            CanonicalRepresentation = collector.CanonicalRepresentation;
            TerminalPropCaptureGroupName = terminalPropCaptureGroupName;
            TotalCount = collector.VariantCounts.Sum(x => x.Value);
            VariantCounts = collector.VariantCounts.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        public ValueCaptureVariantSet(string canonicalRepresentation, string terminalPropCaptureGroupName)
        {
            // Used for zero-occurrence count enum members

            CanonicalRepresentation = canonicalRepresentation;
            TerminalPropCaptureGroupName = terminalPropCaptureGroupName;
            TotalCount = 0;
            VariantCounts.Add(CanonicalRepresentation, 0);
        }

        public void SetPrettyRegexCaptureLine(PrettifiedRegex prettifiedRegex)
        {
            var matchedByLine = prettifiedRegex.Lines.FirstOrDefault(x => x.CheckIfMatch(CanonicalRepresentation, TerminalPropCaptureGroupName));

            if (matchedByLine != null)
                CapturedByPrettyRegexLine = matchedByLine.LineNumber;
        }

        public override string ToString()
        {
            var mainString = $"{CanonicalRepresentation}: {TotalCount}";

            if (!VariantCounts.Select(x => x.Key).Any(x => x != CanonicalRepresentation))
                return mainString;

            var subPartStr = string.Join(" | ", VariantCounts.Select(x => $"{x.Key}: {x.Value}"));

            return $"{mainString} ({subPartStr})";
        }
    }
}