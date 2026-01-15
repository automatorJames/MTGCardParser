namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record TokenUnitCapture
{
    public Type Type { get; }
    public string TypeName { get; }
    public string TypeNameFriendly { get; }
    public int OccurrenceCount { get; }
    public List<RegexCommentedLine> FilteredLines { get; }
    public string MinifiedRegexString { get; }
    public string FormattedRegexString { get; }

    /// <summary>
    /// Maps prop path to parent of terminal --> set of capture value variant counts.
    /// </summary>
    public Dictionary<CaptureGroupPropPath, PropPathSynonymSetContainer> PropPathVariantSets { get; private set; } = [];

    public TokenUnitCapture(Type type, List<TokenUnit> rootTokensUnitsOfType = null)
    {
        Type = type;
        TypeName = type.Name;
        TypeNameFriendly = TypeName.ToFriendlyCase(TitleDisplayOption.Sentence);
        var template = TokenTypeRegistry.Templates[type];

        if (rootTokensUnitsOfType != null)
        {
            ProcessFlattenedTerminalCaptures(rootTokensUnitsOfType);
            FilteredLines = template.Builder.GetFormattedLines(PropPathVariantSets.Values.ToList());
            OccurrenceCount = rootTokensUnitsOfType.Count;
        }
        else
            FilteredLines = template.Builder.GetFormattedLines(null);

        FormattedRegexString = string.Join("\r\n", FilteredLines.Select(x => x.FormattedText));
        MinifiedRegexString = template.Builder.GetMinified();
    }

    void ProcessFlattenedTerminalCaptures(List<TokenUnit> rootTokensUnitsOfType)
    {
        foreach (var tokenUnit in rootTokensUnitsOfType)
        {
            var flattenedTerminalCaptures = tokenUnit.GetFlattenedTerminalCaptures();

            foreach (var capture in flattenedTerminalCaptures)
            {
                var parentPropPath = capture.CaptureGroupPropPath.Parent
                    ?? throw new Exception($"The path {capture.CaptureGroupPropPath?.PropPath} has no parent, but one was expected");

                if (!PropPathVariantSets.TryGetValue(parentPropPath, out var propPathVariantSetWrapper))
                {
                    propPathVariantSetWrapper = new(parentPropPath, capture.TemplatePropInfo);
                    PropPathVariantSets[parentPropPath] = propPathVariantSetWrapper;
                }

                if (!propPathVariantSetWrapper.SynonymSets.TryGetValue(capture.Value, out var captureValueVariantSet))
                {
                    captureValueVariantSet = new(capture.Value, capture.Capture);
                    propPathVariantSetWrapper.SynonymSets[capture.Value] = captureValueVariantSet;
                }
                else
                    captureValueVariantSet.IncrementSynonymCapture(capture.Capture);
            }
        }

        PropPathVariantSets.Values.ToList().ForEach(x => x.OrderByOccurrenceCount());
    }
}