namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

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
        RegexString = TokenTypeRegistry.Templates[type].RegexString;

        if (collectors != null)
        {
            foreach (var propPathValSetCollector in collectors)
            {
                var variantSets = propPathValSetCollector.Value.Values
                    .Select(x => new ValueCaptureVariantSet(x, propPathValSetCollector.Key.TerminalPropName))
                    .OrderByDescending(x => x.TotalCount)
                    .ToList();

                // If enum, populate any missing zero-match members
                if (propPathValSetCollector.Key.Prop.RegexPropType == RegexPropType.Enum)
                {
                    var valuesWithCounts = propPathValSetCollector.Value.Keys.ToList();

                    var missingZeroCountEnumValStrings = Enum.GetValues(propPathValSetCollector.Key.Prop.UnderlyingType)
                        .Cast<object>()
                        .Select(x => x.ToString().ToFriendlyCase(TitleDisplayOption.Lower))
                        .Except(valuesWithCounts)
                        .ToList();

                    foreach (var missingItem in missingZeroCountEnumValStrings)
                        variantSets.Add(new ValueCaptureVariantSet(missingItem, propPathValSetCollector.Key.TerminalPropName));
                }

                var (captureGroupStart, captureGroupEnd) = FindNamedCaptureGroupSpan(propPathValSetCollector.Key.TerminalPropName);
                var regexPropValueSet = new RegexPropValueSet(propPathValSetCollector.Key, captureGroupStart, captureGroupEnd, variantSets);
                RegexPropValueSets.Add(regexPropValueSet);
            }
        }

        PrettifiedRegex = new PrettifiedRegex(RegexString, type);
        RegexPropValueSets.ForEach(x => x.SetPrettyRegexCaptureLineAll(PrettifiedRegex));
    }

    (int start, int endExclusive) FindNamedCaptureGroupSpan(string name)
    {
        var regex = new Regex(
            $@"\(\?<{Regex.Escape(name)}>(?:[^()]+|\((?<DEPTH>)|\)(?<-DEPTH>))*(?(DEPTH)(?!))\)",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

        var match = regex.Match(RegexString);
        return match.Success ? (match.Index, match.Index + match.Length) : (-1, -1);
    }
}
