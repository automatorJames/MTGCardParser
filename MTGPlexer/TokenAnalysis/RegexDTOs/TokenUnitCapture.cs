namespace MTGPlexer.TokenAnalysis.RegexDTOs;

public record TokenUnitCapture
{
    public Type Type { get; }
    public int OccurrenceCount { get; }
    public string RegexString { get; }
    public PrettifiedRegex PrettifiedRegex { get; }
    public List<RegexPropValueSet> RegexPropValueSets { get; } = [];
    public DeterministicPalette Palette { get; }

    public TokenUnitCapture(Type type, int occurrenceCount, Dictionary<TerminalRegexPropPath, Dictionary<string, ValueCaptureVariantCollector>> collectors = null)
    {
        Type = type;
        OccurrenceCount = occurrenceCount;
        Palette = TokenTypeRegistry.Palettes[type];
        RegexString = TokenTypeRegistry.Templates[type].RenderedRegexString;

        if (collectors != null)
        {
            foreach (var propPathValSetCollector in collectors)
            {
                var variantSets = propPathValSetCollector.Value.Values
                    .Select(x => new ValueCaptureVariantSet(x, RegexString, propPathValSetCollector.Key.TerminalPropName))
                    .ToList();

                var (captureGroupStart, captureGroupEnd) = FindNamedCaptureGroupSpan(propPathValSetCollector.Key.TerminalPropName);
                RegexPropValueSets.Add(new RegexPropValueSet(propPathValSetCollector.Key, captureGroupStart, captureGroupEnd, variantSets));
            }
        }

        PrettifiedRegex = new PrettifiedRegex(RegexString, type);
    }

    (int start, int endExclusive) FindNamedCaptureGroupSpan(string name)
    {
        // This regex is simplified for correctness in finding the span of a balanced group.
        var regex = new Regex(
            $@"\(\?<{Regex.Escape(name)}>(?:[^()]+|\((?<DEPTH>)|\)(?<-DEPTH>))*(?(DEPTH)(?!))\)",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

        var match = regex.Match(RegexString);
        return match.Success ? (match.Index, match.Index + match.Length) : (-1, -1);
    }
}
