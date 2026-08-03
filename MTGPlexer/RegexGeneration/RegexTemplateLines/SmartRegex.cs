namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public class SmartRegex
{
    public List<SmartLine> Lines { get; private set; }

    public SmartRegex(List<RegexBrick> bricks, TokenOccurrenceSummary summary, RegexGraph regexGraph)
    {
        var spacedFilteredBricks = GetSpacedFilteredRegexBricks(bricks, summary, regexGraph);
        Lines = SmartLineFactory.Get(spacedFilteredBricks, regexGraph);
    }

    List<RegexBrick> GetSpacedFilteredRegexBricks(List<RegexBrick> bricks, TokenOccurrenceSummary summary, RegexGraph regexGraph)
    {
        List<RegexBrick> finalizedBricks = [];
        var nonEnumMemberRegexBricks = bricks
            .Except(bricks.OfType<RegexBrickValue>()) // Exclude enum members (RegexBrickValue)
            .Except(bricks.OfType<RegexBrickJoiner>().Where(x => x.NamedGroupParent is EnumNode)) // Exclude joiner pipes between enum members
            .ToList();

        for (int i = 0; i < nonEnumMemberRegexBricks.Count; i++)
        {
            var brick = nonEnumMemberRegexBricks[i];
            var regex = brick.Regex;

            bool shouldInsertBlankBefore =
                i > 0 // no blank spaces before first line
                && nonEnumMemberRegexBricks[i - 1] is not RegexBrickGroupOpen // no blank spaces after group opens
                && !(brick is RegexBrickGroupClose && nonEnumMemberRegexBricks[i - 1] is RegexBrickGroupClose); // no blank spaces between two group closes

            if (shouldInsertBlankBefore)
                finalizedBricks.Add(new RegexBrickBlank(regexGraph.RootNode));

            finalizedBricks.Add(brick);

            if (brick is RegexBrickGroupOpen regexBrickGroupOpen)
            {
                regexBrickGroupOpen.SetFormattedGroupName(regexGraph.SimpleUniqueNames[brick.FullyQualifiedName]);

                if (brick.NamedGroupParent is EnumNode enumNode)
                {
                    var enumSummary = summary.EnumCaptureSummaries[enumNode.FullyQualifiedName];

                    var countOfMembersWithZeroOccurrences = enumSummary.EnumMembersWithZeroOcurrences.Count;
                    bool allAreOmitted = countOfMembersWithZeroOccurrences == enumSummary.EnumTotalMemberCount;
                    RegexBrickOmittedCount regexBrickOmittedCount = null;

                    if (countOfMembersWithZeroOccurrences > 0)
                        regexBrickOmittedCount = new RegexBrickOmittedCount(enumNode, countOfMembersWithZeroOccurrences, allAreOmitted);

                    var enumMemberChildrenWithOccurrences = bricks
                        .OfType<RegexBrickValue>()
                        .Where(x => x.NamedGroupLineageNames.Contains(enumNode.FullyQualifiedName))
                        .Where(x => !enumSummary.EnumMembersWithZeroOcurrences.Contains(x.Value))
                        .Where(x => enumSummary.EnumMemberSynonymOccurenceCounts[x.Value].ContainsKey(x.Regex))
                        .OrderByDescending(x => enumSummary.EnumMemberOccurenceCounts[x.Value])
                        .ToList();

                    var colonBuffer = string.Empty.PadLeft(SmartRegexStaticRules.EnumMemberOccurrenceCountColonBuffer);

                    // Widest enum member name and widest occurrence count, so every member's
                    // name is right-aligned against the same colon column, and every member's
                    // occurrence count is left-aligned (first digit aligned) after that colon.
                    var maxNameLength = enumMemberChildrenWithOccurrences.Max(x => x.Value.ToString().Length);
                    var maxDigitLength = enumMemberChildrenWithOccurrences.Max(x => enumSummary.EnumMemberOccurenceCounts[x.Value].ToString().Length);
                    var coreLength = maxNameLength + colonBuffer.Length + 1 + colonBuffer.Length + maxDigitLength;

                    // The comment in the omitted count line (if any was created) may be longer than any other comment
                    var maxCommentLength = Math.Max(coreLength, regexBrickOmittedCount?.Comment.Length ?? 0);

                    RegexNode currentSynonymGroupParentNode = null;

                    for (int j = 0; j < enumMemberChildrenWithOccurrences.Count; j++)
                    {
                        var member = enumMemberChildrenWithOccurrences[j];
                        var occurrences = enumSummary.EnumMemberOccurenceCounts[member.Value];
                        var synonymOccurrences = enumSummary.EnumMemberSynonymOccurenceCounts[member.Value][member.Regex];
                        bool isPartOfSynonymGroup = enumSummary.EnumMemberSynonymOccurenceCounts[member.Value].Count > 1;

                        // The name column is left blank (but still reserved) for synonym rows, so
                        // the colon stays aligned with the header's and other members' colon column.
                        var namedCore = $"{member.Value.ToString().PadLeft(maxNameLength)}{colonBuffer}:{colonBuffer}{occurrences.ToString().PadRight(maxDigitLength)}";
                        var minimalCore = $"{string.Empty.PadLeft(maxNameLength)}{colonBuffer}:{colonBuffer}{synonymOccurrences.ToString().PadRight(maxDigitLength)}";

                        if (enumSummary.MemberIsFirstAmongManyRepresentedSynonyms(member.Value, member.Regex))
                        {
                            currentSynonymGroupParentNode = member.Parent;

                            var header = new RegexBrickSynonymSectionHeader(currentSynonymGroupParentNode, namedCore)
                            {
                                CommentFormatted = SmartRegexStaticRules.CenterPad(namedCore, maxCommentLength)
                            };

                            finalizedBricks.Add(header);
                        }

                        member.CommentFormatted = SmartRegexStaticRules.CenterPad(
                            isPartOfSynonymGroup ? minimalCore : namedCore,
                            maxCommentLength);

                        var pipeBuffer = string.Empty.PadLeft(SmartRegexStaticRules.EnumMemberBufferAfterPipe);
                        member.RegexFormatted = $"{(j == 0 ? " " : "|")}{pipeBuffer}{member.Regex}";

                        finalizedBricks.Add(member);

                        bool isLastOfSynonymGroup =
                            isPartOfSynonymGroup
                            && (j == enumMemberChildrenWithOccurrences.Count - 1
                                || !Equals(enumMemberChildrenWithOccurrences[j + 1].Value, member.Value));

                        if (isLastOfSynonymGroup)
                        {
                            bool isLastMemberBrickForEnum = j == enumMemberChildrenWithOccurrences.Count - 1;

                            if (!isLastMemberBrickForEnum)
                                finalizedBricks.Add(new RegexBrickSynonymSectionFooter(currentSynonymGroupParentNode, maxCommentLength));
                        }
                    }

                    if (countOfMembersWithZeroOccurrences > 0)
                    {
                        regexBrickOmittedCount.CommentFormatted = SmartRegexStaticRules.CenterPad(regexBrickOmittedCount.Comment, maxCommentLength);
                        finalizedBricks.Add(regexBrickOmittedCount);
                    }
                }
            }
        }

        return finalizedBricks;
    }

    public override string ToString() =>
        string.Join("\r\n", Lines);
}