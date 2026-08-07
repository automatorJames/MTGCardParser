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
    /// <param name="includeOmittedCount">Whether to append the trailing "N omitted" summary row at all.</param>
    public List<RegexBrick> Build(EnumNode enumNode, List<RegexBrick> allBricks, EnumCaptureTraceSummary enumSummary, bool includeOmittedCount = true)
    {
        var members = GetOccurringMembersOrderedByFrequency(enumNode, allBricks, enumSummary);
        var omittedCount = includeOmittedCount ? BuildOmittedCountBrick(enumNode, enumSummary) : null;
        var metrics = EnumColumnMetrics.Calculate(members, enumSummary, omittedCount);

        List<RegexBrick> bricks = [];
        AppendMemberBricksWithSynonymSections(bricks, members, enumSummary, metrics);

        if (omittedCount != null)
            bricks.Add(omittedCount);

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

            bool isFirstRowOfSynonymGroup =
                isPartOfSynonymGroup
                && (i == 0 || !Equals(members[i - 1].Value, member.Value));

            if (isFirstRowOfSynonymGroup)
            {
                currentSynonymGroupParentNode = member.Parent;
                bricks.Add(BuildSynonymSectionHeader(currentSynonymGroupParentNode, member, enumSummary, metrics));
            }

            member.IsSynonymRow = isPartOfSynonymGroup;
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
        return new RegexBrickSynonymSectionHeader(parent, namedCore);
    }

    /// <summary>
    /// The uncentered "Name : count" comment core for a standalone member, or "     : count" for one row of a
    /// synonym group. Centering against the box's actual width happens later, in <see cref="SmartLineRenderer"/>,
    /// once that width is known across the whole formatted regex. Splits the result across
    /// <see cref="RegexBrickValue.NameCommentFormatted"/> and <see cref="RegexBrickValue.CountCommentFormatted"/>
    /// (which together concatenate to the same string this always returned) so the name and count can be
    /// colored as separate spans.
    /// </summary>
    static string BuildMemberComment(RegexBrickValue member, EnumCaptureTraceSummary enumSummary, EnumColumnMetrics metrics, bool isPartOfSynonymGroup)
    {
        var nameField = isPartOfSynonymGroup ? metrics.FormatBlankNameField() : metrics.FormatNameField(member.Value);

        var occurrenceCount = isPartOfSynonymGroup
            ? enumSummary.EnumMemberSynonymOccurenceCounts[member.Value][member.Regex]
            : enumSummary.EnumMemberOccurenceCounts[member.Value];
        var countField = metrics.FormatCountField(occurrenceCount);

        member.NameCommentFormatted = nameField;
        member.CountCommentFormatted = countField;

        return member.NameCommentFormatted + member.CountCommentFormatted;
    }

    /// <summary>
    /// The member's regex prefixed with a leading space (first row) or pipe (every subsequent row). Splits
    /// the result across <see cref="RegexBrickValue.JoinerRegexFormatted"/> and <see cref="RegexBrickValue.MemberRegexFormatted"/>
    /// so the joiner and pattern text can be colored as separate spans.
    /// </summary>
    static string BuildMemberRegex(RegexBrickValue member, int positionAmongOccurring)
    {
        var pipeBuffer = string.Empty.PadLeft(SmartRegexStaticRules.EnumMemberBufferAfterPipe);
        var prefix = positionAmongOccurring == 0 ? " " : "|";

        member.JoinerRegexFormatted = $"{prefix}{pipeBuffer}";
        member.MemberRegexFormatted = member.Regex;

        return member.JoinerRegexFormatted + member.MemberRegexFormatted;
    }
}
