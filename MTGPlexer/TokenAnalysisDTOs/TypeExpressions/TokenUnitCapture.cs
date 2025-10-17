namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record TokenUnitCapture
{
    public Type Type { get; }
    public string TypeName { get; }
    public string TypeNameFriendly { get; }
    public int OccurrenceCount { get; }
    public List<RegexCommentedLine> FilteredLines { get; }
    public Palette Palette { get; }
    public HashSet<CaptureGroupPropPath> MatchedAlternatePaths { get; } = [];
    public string MinifiedRegexString { get; }
    public string FormattedRegexString { get; }

    /// <summary>
    /// Maps path to terminal prop (not including value) --> set of capture value variant counts
    /// </summary>
    public Dictionary<CaptureGroupPropPath, PropPathSynonymSetWrapper> PropPathVariantSetWrappers { get; } = [];

    public TokenUnitCapture(Type type, List<TokenUnit> rootTokensUnitsOfType)
    {
        Type = type;
        TypeName = type.Name;
        TypeNameFriendly = TypeName.ToFriendlyCase(TitleDisplayOption.Sentence);
        Palette = TokenTypeRegistry.Palettes[type];
        var template = TokenTypeRegistry.Templates[type];
        OccurrenceCount = rootTokensUnitsOfType.Count;

        foreach (var tokenUnit in rootTokensUnitsOfType)
            foreach (var flattenedTerminalCapture in tokenUnit.GetFlattenedTerminalCaptures())
            {
                var propPath = flattenedTerminalCapture.CaptureGroupPropPath.Parent;

                if (propPath == null)
                    throw new Exception($"The path {flattenedTerminalCapture.CaptureGroupPropPath?.PropPath} has no parent, but one was expected");

                if (!PropPathVariantSetWrappers.TryGetValue(propPath, out var propPathVariantSetWrapper))
                {
                    propPathVariantSetWrapper = new(propPath, flattenedTerminalCapture.RegexPropInfo);
                    PropPathVariantSetWrappers[propPath] = propPathVariantSetWrapper;
                }

                if (!propPathVariantSetWrapper.SynonymSets.TryGetValue(flattenedTerminalCapture.Value, out var captureValueVariantSet))
                {
                    captureValueVariantSet = new(flattenedTerminalCapture.Value, flattenedTerminalCapture.Capture);
                    propPathVariantSetWrapper.SynonymSets[flattenedTerminalCapture.Value] = captureValueVariantSet;
                }
                else
                    captureValueVariantSet.IncrementSynonymCapture(flattenedTerminalCapture.Capture);

                MatchedAlternatePaths.Add(flattenedTerminalCapture.CaptureGroupPropPath);
            }

        PropPathVariantSetWrappers.Values.ToList().ForEach(x => x.OrderByOccurrenceCount());

        FilteredLines = template.Builder.GetFormattedLines(PropPathVariantSetWrappers.Values.ToList());

        // Todo: the formatting here isn't exactly right for either of these
        FormattedRegexString = string.Join("\r\n", FilteredLines.Select(x => x.FormattedText));
        MinifiedRegexString = string.Join("", FilteredLines.Select(x => x.Regex.Trim()));
    }
}