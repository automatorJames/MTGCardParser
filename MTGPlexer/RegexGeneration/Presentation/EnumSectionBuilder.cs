namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>
/// Builds the bricks that represent one enum group's members once formatting reaches that group's
/// opening brick: occurrence-ranked member rows, synonym rows grouped under a shared header/footer when
/// a member has more than one represented synonym, and a trailing "N omitted" row for members that
/// never occurred in the analyzed data. This is the enum-specific formatting logic that used to live
/// inline inside <c>SmartRegex.GetSpacedFilteredRegexBricks</c>.
/// </summary>
internal class EnumSectionBuilder
{
    /// <summary>Builds the full ordered sequence of member/synonym/omitted-count bricks for <paramref name="enumNode"/>.</summary>
    public List<RegexBrick> Build(EnumNode enumNode, List<RegexBrick> allBricks, EnumCaptureTraceSummary enumSummary)
    {
        var members = GetOccurringMembersOrderedByFrequency(enumNode, allBricks, enumSummary);
        var omittedCount = BuildOmittedCountBrick(enumNode, enumSummary);
        var metrics = EnumColumnMetrics.Calculate(members, enumSummary, omittedCount);

        List<RegexBrick> bricks = [];
        AppendMemberBricksWithSynonymSections(bricks, members, enumSummary, metrics);

        if (omittedCount != null)
        {
            omittedCount.CommentFormatted = SmartRegexStaticRules.CenterPad(omittedCount.CommentFormatted, metrics.MaxCommentLength);
            bricks.Add(omittedCount);
        }

        return bricks;
    }

    /// <summary>
    /// The enum's member value bricks that actually occurred in the analyzed data (excluding
    /// synonym patterns that never occurred), ordered by total occurrence count, most frequent first.
    /// </summary>
    static List<RegexBrickValue> GetOccurringMembersOrderedByFrequency(EnumNode enumNode, List<RegexBrick> allBricks, EnumCaptureTraceSummary enumSummary) =>
        allBricks
            .OfType<RegexBrickValue>()
            .Where(x => x.NamedGroupLineageNames.Contains(enumNode.FullyQualifiedName))
            .Where(x => !enumSummary.EnumMembersWithZeroOcurrences.Contains(x.Value))
            .Where(x => enumSummary.EnumMemberSynonymOccurenceCounts[x.Value].ContainsKey(x.Regex))
            .OrderByDescending(x => enumSummary.EnumMemberOccurenceCounts[x.Value])
            .ToList();

    /// <summary>Builds the "N omitted" / "All N omitted" summary brick, or null if every member occurred at least once.</summary>
    static RegexBrickOmittedCount BuildOmittedCountBrick(EnumNode enumNode, EnumCaptureTraceSummary enumSummary)
    {
        var zeroOccurrenceCount = enumSummary.EnumMembersWithZeroOcurrences.Count;

        if (zeroOccurrenceCount == 0)
            return null;

        bool allAreOmitted = zeroOccurrenceCount == enumSummary.EnumTotalMemberCount;
        return new RegexBrickOmittedCount(enumNode, zeroOccurrenceCount, allAreOmitted);
    }

    /// <summary>
    /// Walks <paramref name="members"/> in frequency order, inserting a synonym section header before the
    /// first row of any member with multiple represented synonyms, and a divider footer after its last row.
    /// </summary>
    static void AppendMemberBricksWithSynonymSections(List<RegexBrick> bricks, List<RegexBrickValue> members, EnumCaptureTraceSummary enumSummary, EnumColumnMetrics metrics)
    {
        RegexNode currentSynonymGroupParentNode = null;

        for (int i = 0; i < members.Count; i++)
        {
            var member = members[i];
            bool isPartOfSynonymGroup = enumSummary.EnumMemberSynonymOccurenceCounts[member.Value].Count > 1;

            if (enumSummary.MemberIsFirstAmongManyRepresentedSynonyms(member.Value, member.Regex))
            {
                currentSynonymGroupParentNode = member.Parent;
                bricks.Add(BuildSynonymSectionHeader(currentSynonymGroupParentNode, member, enumSummary, metrics));
            }

            member.CommentFormatted = BuildMemberComment(member, enumSummary, metrics, isPartOfSynonymGroup);
            member.RegexFormatted = BuildMemberRegex(member, positionAmongOccurring: i);
            bricks.Add(member);

            bool isLastRowOfSynonymGroup =
                isPartOfSynonymGroup
                && (i == members.Count - 1 || !Equals(members[i + 1].Value, member.Value));

            if (isLastRowOfSynonymGroup && i != members.Count - 1)
                bricks.Add(new RegexBrickSynonymSectionFooter(currentSynonymGroupParentNode, metrics.MaxCommentLength));
        }
    }

    static RegexBrickSynonymSectionHeader BuildSynonymSectionHeader(RegexNode parent, RegexBrickValue member, EnumCaptureTraceSummary enumSummary, EnumColumnMetrics metrics)
    {
        var namedCore = metrics.FormatNamedCore(member.Value, enumSummary.EnumMemberOccurenceCounts[member.Value]);
        return new RegexBrickSynonymSectionHeader(parent, namedCore) { CommentFormatted = SmartRegexStaticRules.CenterPad(namedCore, metrics.MaxCommentLength) };
    }

    /// <summary>The centered "Name : count" comment for a standalone member, or "     : count" for one row of a synonym group.</summary>
    static string BuildMemberComment(RegexBrickValue member, EnumCaptureTraceSummary enumSummary, EnumColumnMetrics metrics, bool isPartOfSynonymGroup)
    {
        var core = isPartOfSynonymGroup
            ? metrics.FormatMinimalCore(enumSummary.EnumMemberSynonymOccurenceCounts[member.Value][member.Regex])
            : metrics.FormatNamedCore(member.Value, enumSummary.EnumMemberOccurenceCounts[member.Value]);

        return SmartRegexStaticRules.CenterPad(core, metrics.MaxCommentLength);
    }

    /// <summary>The member's regex prefixed with a leading space (first row) or pipe (every subsequent row).</summary>
    static string BuildMemberRegex(RegexBrickValue member, int positionAmongOccurring)
    {
        var pipeBuffer = string.Empty.PadLeft(SmartRegexStaticRules.EnumMemberBufferAfterPipe);
        var prefix = positionAmongOccurring == 0 ? " " : "|";
        return $"{prefix}{pipeBuffer}{member.Regex}";
    }
}
