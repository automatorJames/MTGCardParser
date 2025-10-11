namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record TokenUnitCapture
{
    public Type Type { get; }
    public string TypeName { get; }
    public string TypeNameFriendly { get; }
    public int OccurrenceCount { get; }
    //public FormattedRegex FormattedRegex { get; }
    public List<RegexCommentedLine> FilteredLines { get; }
    public Palette Palette { get; }
    public HashSet<CaptureGroupPropPath> MatchedAlternatePaths { get; } = [];
    public string MinifiedRegexString { get; }
    public string FormattedRegexString { get; }

    /// <summary>
    /// Maps path to terminal prop (not including value) --> set of capture value variant counts
    /// </summary>
    public Dictionary<CaptureGroupPropPath, Dictionary<object, CaptureValueVariantSet>> PropPathVariantSets { get; } = [];

    public TokenUnitCapture(Type type, List<TokenUnit> rootTokensUnitsOfType)
    {
        Type = type;
        TypeName = type.Name;
        TypeNameFriendly = TypeName.ToFriendlyCase(TitleDisplayOption.Sentence);
        Palette = TokenTypeRegistry.Palettes[type];
        var template = TokenTypeRegistry.Templates[type];
        //FormattedRegex = template.FormattedRegex;
        OccurrenceCount = rootTokensUnitsOfType.Count;

        foreach (var tokenUnit in rootTokensUnitsOfType)
            foreach (var flattenedTerminalCapture in tokenUnit.GetFlattenedTerminalCaptures())
            {
                var propPath = flattenedTerminalCapture.CaptureGroupPropPath.Parent;

                if (propPath == null)
                    throw new Exception($"The path {flattenedTerminalCapture.CaptureGroupPropPath?.PropPath} has no parent, but one was expected");

                if (!PropPathVariantSets.TryGetValue(propPath, out var variantSetDict))
                {
                    variantSetDict = [];
                    PropPathVariantSets[propPath] = variantSetDict;
                }

                if (!variantSetDict.TryGetValue(flattenedTerminalCapture.Value, out var captureValueVariantSet))
                {
                    captureValueVariantSet = new(flattenedTerminalCapture.Value, flattenedTerminalCapture.Capture);
                    variantSetDict[flattenedTerminalCapture.Value] = captureValueVariantSet;
                }
                else
                    captureValueVariantSet.IncrementVariantCapture(flattenedTerminalCapture.Capture);

                MatchedAlternatePaths.Add(flattenedTerminalCapture.CaptureGroupPropPath);
            }

        FilteredLines = template.Collector.GetFormattedLines(MatchedAlternatePaths);

        // Todo: the formatting here isn't exactly right for either of these
        FormattedRegexString = string.Join("\r\n", FilteredLines.Select(x => x.FormattedText));
        MinifiedRegexString = string.Join("", FilteredLines.Select(x => x.Regex.Trim()));

        PropPathVariantSets = PropPathVariantSets
            .ToDictionary(
                x => x.Key,
                x => x.Value
                    .OrderByDescending(inner => inner.Value.TotalCount)
                    .ToDictionary(inner => inner.Key, inner => inner.Value)
            );
    }
}