namespace Glyphotype.RegexGeneration.Presentation;

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
    public List<RegexBrick> Build(EnumNode enumNode, List<RegexBrick> allBricks, EnumCaptureTraceSummary enumSummary, RegexDisplayMode displayMode, bool includeOmittedCount = true)
    {
        var members = GetMembersToDisplay(enumNode, allBricks, enumSummary, displayMode);

        // Full already shows every member with its real (possibly zero) count, so there's nothing left
        // for an "N omitted" row to summarize.
        var omittedCount = includeOmittedCount && displayMode != RegexDisplayMode.Full
            ? BuildOmittedCountBrick(enumNode, enumSummary)
            : null;
        var metrics = EnumColumnMetrics.Calculate(members, enumSummary, omittedCount);

        List<RegexBrick> bricks = [];
        AppendMemberBricksWithSynonymSections(bricks, members, enumSummary, metrics, displayMode);

        if (omittedCount != null)
            bricks.Add(omittedCount);

        return bricks;
    }

    /// <summary>
    /// The enum's member value bricks to render, selected and ordered per <paramref name="displayMode"/>:
    /// <list type="bullet">
    /// <item><see cref="RegexDisplayMode.Full"/> - every declared member, alphabetically; a member that
    /// never occurred gets a single representative row (its first declared pattern) standing in for the
    /// whole (empty) synonym set, since there's no occurrence data to break it down by pattern.</item>
    /// <item><see cref="RegexDisplayMode.Sample"/> - up to three members, favoring ones that occurred
    /// (ranked by occurrence count then alphabetically, one representative row per member regardless of
    /// how many of its synonym patterns occurred), then backfilled alphabetically from members that never
    /// occurred if fewer than three did.</item>
    /// <item>Anything else (<see cref="RegexDisplayMode.MatchedOnly"/>) - every occurring member, full
    /// synonym breakdown included, ranked by occurrence count - today's default view.</item>
    /// </list>
    /// </summary>
    static List<RegexBrickValue> GetMembersToDisplay(EnumNode enumNode, List<RegexBrick> allBricks, EnumCaptureTraceSummary enumSummary, RegexDisplayMode displayMode)
    {
        // Checked via EnumCaptureTraceSummary's safe accessors rather than the raw dictionaries/
        // EnumMembersWithZeroOcurrences: a member the graph declares but the summary has no entry for at
        // all (e.g. a dynamically-generated enum whose member set has since moved on from what this
        // RegexGraph was built against) isn't in either, and treating "not in the zero list" as "occurred"
        // used to send it straight into a raw dictionary indexer with no entry there either - a
        // KeyNotFoundException. Missing from the summary now just reads as "didn't occur," the same safe
        // default as an explicit zero.
        bool Occurred(object memberValue) => enumSummary.GetOccurrenceCount(memberValue) > 0;
        bool IsOccurringPattern(RegexBrickValue member) => enumSummary.GetSynonymOccurrenceCounts(member.Value).ContainsKey(member.Regex);

        // A member the summary counts as "occurred" isn't guaranteed to have a declared pattern brick
        // whose raw Regex text exactly matches one of its recorded capture strings (e.g. a pattern with
        // its own internal variability) - falling back to the group's first brick rather than demanding
        // a match keeps this a representative-picker, not a filter that can come up empty.
        RegexBrickValue PickRepresentative(IGrouping<object, RegexBrickValue> group) =>
            group.FirstOrDefault(IsOccurringPattern) ?? group.First();

        var membersByValue = allBricks
            .OfType<RegexBrickValue>()
            .Where(x => x.NamedGroupLineageNames.Contains(enumNode.FullyQualifiedName))
            .GroupBy(x => x.Value);

        if (displayMode == RegexDisplayMode.Full)
            return membersByValue
                .SelectMany(group => Occurred(group.Key) ? group.Where(IsOccurringPattern) : [group.First()])
                .OrderBy(x => x.Value.ToString(), StringComparer.OrdinalIgnoreCase)
                .ToList();

        if (displayMode == RegexDisplayMode.Sample)
        {
            var occurring = membersByValue
                .Where(group => Occurred(group.Key))
                .Select(PickRepresentative)
                .OrderByDescending(x => enumSummary.GetOccurrenceCount(x.Value))
                .ThenBy(x => x.Value.ToString(), StringComparer.OrdinalIgnoreCase)
                .Take(3)
                .ToList();

            // Nothing occurred at all (or fewer than three members did) - rather than show an
            // emptier-than-necessary sample, backfill alphabetically from members that never occurred.
            var backfill = membersByValue
                .Where(group => !Occurred(group.Key))
                .Select(group => group.First())
                .OrderBy(x => x.Value.ToString(), StringComparer.OrdinalIgnoreCase)
                .Take(3 - occurring.Count);

            return [.. occurring, .. backfill];
        }

        return membersByValue
            .Where(group => Occurred(group.Key))
            .SelectMany(group => group.Where(IsOccurringPattern))
            .OrderByDescending(x => enumSummary.GetOccurrenceCount(x.Value))
            .ToList();
    }

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
    /// Sample never groups by synonym - it already reduced every member to one representative row, so
    /// every row renders as if standalone regardless of how many synonyms the member actually has.
    /// </summary>
    static void AppendMemberBricksWithSynonymSections(List<RegexBrick> bricks, List<RegexBrickValue> members, EnumCaptureTraceSummary enumSummary, EnumColumnMetrics metrics, RegexDisplayMode displayMode)
    {
        RegexNode currentSynonymGroupParentNode = null;

        for (int i = 0; i < members.Count; i++)
        {
            var member = members[i];
            bool isPartOfSynonymGroup = displayMode != RegexDisplayMode.Sample && enumSummary.GetSynonymOccurrenceCounts(member.Value).Count > 1;

            bool isFirstRowOfSynonymGroup =
                isPartOfSynonymGroup
                && (i == 0 || !Equals(members[i - 1].Value, member.Value));

            if (isFirstRowOfSynonymGroup)
            {
                currentSynonymGroupParentNode = member.Parent;
                bricks.Add(BuildSynonymSectionHeader(currentSynonymGroupParentNode, member, enumSummary, metrics));
            }

            member.CommentFormatted = BuildMemberComment(member, enumSummary, metrics, isPartOfSynonymGroup);
            member.RegexFormatted = BuildMemberRegex(member, positionAmongOccurring: i, totalMemberCount: members.Count);
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
        var nameField = metrics.FormatNameField(member.Value);
        var countField = metrics.FormatCountField(enumSummary.GetOccurrenceCount(member.Value));
        return new RegexBrickSynonymSectionHeader(parent, nameField, countField);
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
            ? enumSummary.GetSynonymOccurrenceCounts(member.Value).GetValueOrDefault(member.Regex, 0)
            : enumSummary.GetOccurrenceCount(member.Value);
        var countField = metrics.FormatCountField(occurrenceCount);

        member.NameCommentFormatted = nameField;
        member.CountCommentFormatted = countField;

        return member.NameCommentFormatted + member.CountCommentFormatted;
    }

    /// <summary>
    /// The member's regex prefixed with a leading space (first row) or pipe (every subsequent row) — or no
    /// joiner at all when it's the only occurring member, since there's no sibling for a "|" to separate it
    /// from and reserving that space just leaves an awkward gap. Splits the result across
    /// <see cref="RegexBrickValue.JoinerRegexFormatted"/> and <see cref="RegexBrickValue.MemberRegexFormatted"/>
    /// so the joiner and pattern text can be colored as separate spans.
    /// </summary>
    static string BuildMemberRegex(RegexBrickValue member, int positionAmongOccurring, int totalMemberCount)
    {
        member.JoinerRegexFormatted = BuildJoinerPrefix(positionAmongOccurring, totalMemberCount);
        // Escaped for display only - member.Regex itself must stay raw, since it's the key looked up against
        // EnumCaptureTraceSummary's synonym table (see IsOccurringPattern), which is keyed by matched
        // document text rather than by pattern text.
        member.MemberRegexFormatted = BuiltRegex.EscapeSpaces(member.Regex);

        return member.JoinerRegexFormatted + member.MemberRegexFormatted;
    }

    static string BuildJoinerPrefix(int positionAmongOccurring, int totalMemberCount)
    {
        if (totalMemberCount == 1)
            return "";

        var pipeBuffer = string.Empty.PadLeft(SmartRegexStaticRules.EnumMemberBufferAfterPipe);
        var prefix = positionAmongOccurring == 0 ? " " : "|";

        return $"{prefix}{pipeBuffer}";
    }
}
