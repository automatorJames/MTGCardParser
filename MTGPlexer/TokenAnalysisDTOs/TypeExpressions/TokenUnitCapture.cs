namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record TokenUnitCapture
{
    public Type Type { get; }
    public string TypeName { get; }
    public string TypeNameFriendly { get; }
    public int OccurrenceCount { get; }
    public List<RegexCommentedLine> CommentedLines { get; }
    public string FormattedRegex { get; }
    public string MinifiedRegexString { get; }
    public List<RegexPropValueSet> RegexPropValueSets { get; } = [];
    public Palette Palette { get; }

    public TokenUnitCapture(Type type, int occurrenceCount, Dictionary<TerminalRegexPropPath, Dictionary<string, ValueCaptureVariantCollector>> collectors = null)
    {
        Type = type;
        TypeName = type.Name;
        TypeNameFriendly = TypeName.ToFriendlyCase(TitleDisplayOption.Sentence);
        OccurrenceCount = occurrenceCount;
        Palette = TokenTypeRegistry.Palettes[type];

        var template = TokenTypeRegistry.Templates[type];
        var generatedRegex = TokenTypeRegistry.Templates[type].GeneratedRegex;
        FormattedRegex = generatedRegex.FormattedRegex;
        CommentedLines = generatedRegex.CommentedLines;
        MinifiedRegexString = generatedRegex.MinifiedRegex;

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

                    var missingZeroCountEnumValStrings = Enum.GetValues(propPathValSetCollector.Key.Prop.BaseType)
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
    }

    (int start, int endExclusive) FindNamedCaptureGroupSpan(string name)
    {
        var regex = new Regex(
            $@"\(\?<{Regex.Escape(name)}>(?:[^()]+|\((?<DEPTH>)|\)(?<-DEPTH>))*(?(DEPTH)(?!))\)",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

        var match = regex.Match(FormattedRegex);
        return match.Success ? (match.Index, match.Index + match.Length) : (-1, -1);
    }
}