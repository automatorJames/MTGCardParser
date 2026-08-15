namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>
/// Builds the bricks that represent one dynamic group's resolved captures once formatting reaches that
/// group's opening brick: for each type resolved at runtime, its distinct captured values ranked by
/// occurrence, with a shared header row (aggregate count) when a type has more than one distinct captured
/// value. Analogous to <see cref="EnumSectionBuilder"/>, except a dynamic group's real matching pattern
/// (<c>[^.]+</c>) can't itself be enumerated the way an enum's members can, so these rows are illustrative
/// examples of what the group actually captured rather than the alternatives it could match. When nothing
/// was captured (e.g. a zero-occurrence type), falls back to rendering the group's raw, unformatted bricks.
/// </summary>
internal class DynamicSectionBuilder
{
    /// <summary>Builds the full ordered sequence of resolved-type/capture-value rows for <paramref name="dynamicNode"/>.</summary>
    public List<RegexBrick> Build(DynamicTokenNode dynamicNode, List<RegexBrick> allBricks, DynamicCaptureTraceSummary dynamicSummary)
    {
        if (dynamicSummary.ResolvedTypeCaptureValueOccurrenceCounts.Count == 0)
            return BuildFallbackBricks(dynamicNode, allBricks);

        var typeGroups = dynamicSummary.ResolvedTypeCaptureValueOccurrenceCounts
            .OrderByDescending(x => x.Value.Values.Sum())
            .ToList();

        var metrics = DynamicColumnMetrics.Calculate(dynamicSummary.ResolvedTypeCaptureValueOccurrenceCounts);

        List<RegexBrick> bricks = [];

        for (int i = 0; i < typeGroups.Count; i++)
        {
            var (type, captureValueCounts) = typeGroups[i];
            bool isLastGroup = i == typeGroups.Count - 1;
            AppendTypeGroupBricks(bricks, dynamicNode, type, captureValueCounts, metrics, isLastGroup);
        }

        return bricks;
    }

    /// <summary>
    /// The group's raw, graph-produced bricks (its literal-match pattern and any inter-pattern joiners),
    /// with their display comments resolved, used when there's no captured data to summarize instead.
    /// </summary>
    static List<RegexBrick> BuildFallbackBricks(DynamicTokenNode dynamicNode, List<RegexBrick> allBricks)
    {
        var rawBricks = allBricks
            .Where(x => x.NamedGroupParent == dynamicNode && (x.Parent is TextNode || x is RegexBrickJoiner))
            .ToList();

        foreach (var brick in rawBricks)
            BrickCommentResolver.Apply(brick);

        return rawBricks;
    }

    /// <summary>
    /// Appends one resolved type's rows: a shared header row (name + aggregate count) when more than one
    /// distinct captured value maps to this type, followed by each captured value ranked by occurrence, and
    /// (when this is such a multi-value group and it isn't the last group in the section) a trailing divider
    /// footer separating it from the next type's rows.
    /// </summary>
    static void AppendTypeGroupBricks(List<RegexBrick> bricks, DynamicTokenNode dynamicNode, Type type, Dictionary<string, int> captureValueCounts, DynamicColumnMetrics metrics, bool isLastGroup)
    {
        var orderedValues = captureValueCounts.OrderByDescending(x => x.Value).ToList();
        var totalCount = captureValueCounts.Values.Sum();
        bool isMultiValue = orderedValues.Count > 1;

        if (isMultiValue)
            bricks.Add(new RegexBrickSynonymSectionHeader(dynamicNode, metrics.FormatNameField(type.Name), metrics.FormatCountField(totalCount)));

        foreach (var (captureValue, count) in orderedValues)
            bricks.Add(BuildValueBrick(dynamicNode, type, captureValue, isMultiValue ? count : totalCount, isMultiValue, metrics));

        if (isMultiValue && !isLastGroup)
            bricks.Add(new RegexBrickSynonymSectionFooter(dynamicNode, metrics.MaxCommentLength));
    }

    static RegexBrickValue BuildValueBrick(DynamicTokenNode dynamicNode, Type type, string captureValue, int occurrenceCount, bool isMultiValue, DynamicColumnMetrics metrics)
    {
        var nameField = isMultiValue ? metrics.FormatBlankNameField() : metrics.FormatNameField(type.Name);
        var countField = metrics.FormatCountField(occurrenceCount);

        return new RegexBrickValue(dynamicNode, captureValue, type)
        {
            NameCommentFormatted = nameField,
            CountCommentFormatted = countField,
            MemberRegexFormatted = captureValue,
            CommentFormatted = nameField + countField,
        };
    }
}
