namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record TokenUnitCapture
{
    int _orphanCaptureCount;

    public Type Type { get; }
    public string TypeName { get; }
    public string TypeNameFriendly { get; }
    public int OccurrenceCount { get; }
    public FormattedRegex FormattedRegex { get; }
    public Palette Palette { get; }
    public HashSet<RegexCommentedAlternateLine> LinesWithMatches { get; } = [];

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
        FormattedRegex = TokenTypeRegistry.Templates[type].FormattedRegex;
        OccurrenceCount = rootTokensUnitsOfType.Count;

        var regexAlternateLines = FormattedRegex.CommentedLines.OfType<RegexCommentedAlternateLine>().ToList();

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

                var matchingRegexAlternateLine = regexAlternateLines.FirstOrDefault(x => x.CaptureGroupPropPath == flattenedTerminalCapture.CaptureGroupPropPath);

                if (matchingRegexAlternateLine != null)
                    LinesWithMatches.Add(matchingRegexAlternateLine);
                else
                    _orphanCaptureCount++;
            }

        PropPathVariantSets = PropPathVariantSets
            .ToDictionary(
                x => x.Key,
                x => x.Value
                    .OrderByDescending(inner => inner.Value.TotalCount)
                    .ToDictionary(inner => inner.Key, inner => inner.Value)
            );
    }
}