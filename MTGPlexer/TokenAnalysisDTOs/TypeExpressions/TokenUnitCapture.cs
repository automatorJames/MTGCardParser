namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record TokenUnitCapture
{
    public Type Type { get; }
    public string TypeName { get; }
    public string TypeNameFriendly { get; }
    public int OccurrenceCount { get; }
    public List<RegexCommentedLine> FilteredLines { get; }
    public Palette Palette { get; }
    public string MinifiedRegexString { get; }
    public string FormattedRegexString { get; }

    /// <summary>
    /// Maps prop path to parent of terminal --> set of capture value variant counts. Includes granualar detail for ManyOfs
    /// so that each captured ManyOf terminal value is represented.
    /// </summary>
    public Dictionary<CaptureGroupPropPath, PropPathSynonymSetContainer> PropPathVariantSetsForRegex { get; private set; } = [];

    /// <summary>
    /// Same as the PropPathVariantSetsForRegex dictionary, except flattens terminal values within each ManyOf so each ManyOf
    /// is displayed as a single entity in the GUI.
    /// </summary>
    public Dictionary<CaptureGroupPropPath, PropPathSynonymSetContainer> PropPathVariantSetsForTable { get; private set; } = [];

    public HashSet<CaptureGroupPropPath> ManyOfItemPropPaths { get; private set; } = [];
    public Dictionary<(Guid distinguisher, ManyOf manyOf), CaptureGroupPropPath> DistinctManyOfValuePropPaths { get; private set; } = [];
    public Dictionary<CaptureGroupPropPath, Dictionary<ManyOf, int>> PropPathToManyOfValueCount { get; private set; } = [];

    public TokenUnitCapture(Type type, List<TokenUnit> rootTokensUnitsOfType = null)
    {
        Type = type;
        TypeName = type.Name;
        TypeNameFriendly = TypeName.ToFriendlyCase(TitleDisplayOption.Sentence);
        Palette = TokenTypeRegistry.Palettes[type];
        var template = TokenTypeRegistry.Templates[type];

        if (rootTokensUnitsOfType != null)
        {
            ProcessFlattenedTerminalCaptures(rootTokensUnitsOfType);
            FilteredLines = template.Builder.GetFormattedLines(PropPathVariantSetsForRegex.Values.ToList());
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
            foreach (var capture in tokenUnit.GetFlattenedTerminalCaptures())
            {
                var parentPropPath = capture.CaptureGroupPropPath.Parent
                    ?? throw new Exception($"The path {capture.CaptureGroupPropPath?.PropPath} has no parent, but one was expected");

                if (!PropPathVariantSetsForRegex.TryGetValue(parentPropPath, out var propPathVariantSetWrapper))
                {
                    propPathVariantSetWrapper = new(parentPropPath, capture.RegexPropInfo);
                    PropPathVariantSetsForRegex[parentPropPath] = propPathVariantSetWrapper;
                }

                if (!propPathVariantSetWrapper.SynonymSets.TryGetValue(capture.Value, out var captureValueVariantSet))
                {
                    captureValueVariantSet = new(capture.Value, capture.Capture);
                    propPathVariantSetWrapper.SynonymSets[capture.Value] = captureValueVariantSet;
                }
                else
                    captureValueVariantSet.IncrementSynonymCapture(capture.Capture);

                // If the parent of this terminal is a ManyOf, save the parent off for later processing and
                // track this path so it's not added to the PropPathVariantSetsForTable dict later
                if (capture.ParentValue is ManyOf manyOf)
                {
                    DistinctManyOfValuePropPaths.TryAdd((manyOf.DistinctId, manyOf), parentPropPath);
                    ManyOfItemPropPaths.Add(capture.CaptureGroupPropPath);
                }
            }
        }

        // Put all the non-ManyOf items from the regex dictionary into the table dictionary
        PropPathVariantSetsForTable = PropPathVariantSetsForRegex
            .Where(x => !ManyOfItemPropPaths.Contains(x.Key))
            .ToDictionary();

        List<(ManyOf manyOf, int count, CaptureGroupPropPath path)> manyOfCounts = DistinctManyOfValuePropPaths
                .GroupBy(x => (x.Key.manyOf, x.Value))
                .Select(x => (x.Key.manyOf, x.Count(), x.Key.Value))
                .ToList();

        PropPathToManyOfValueCount = DistinctManyOfValuePropPaths
            .GroupBy(kvp => kvp.Value)
            .ToDictionary(
                x => x.Key,
                x => x.GroupBy(y => y.Key.manyOf)
                  .ToDictionary(
                      z => z.Key,
                      z => z.Count()
                  )
            );

        // Synthesize wrappers for all ManyOf items and add them to the table dictionary
        foreach ((var path, var manyOfCount) in PropPathToManyOfValueCount)
        {
            PropPathSynonymSetContainer wrapper = new(path);
            PropPathVariantSetsForTable[path] = wrapper;

            foreach ((ManyOf manyOf, int count) in manyOfCount)
                wrapper.SynonymSets.Add(manyOf, new(manyOf, manyOfRelatedPaths: manyOf.GetJoinedPathForAllTerminals(path), count: count));
        }

        // Order values in both dictionaries by descending occurrence count
        PropPathVariantSetsForRegex.Values.ToList().ForEach(x => x.OrderByOccurrenceCount());
        PropPathVariantSetsForTable.Values.ToList().ForEach(x => x.OrderByOccurrenceCount());
    }
}