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

                    var enumMemberChildren = bricks
                        .OfType<RegexBrickValue>()
                        .Where(x => x.NamedGroupLineageNames.Contains(enumNode.FullyQualifiedName))
                        .OrderByDescending(x => enumSummary.EnumMemberOccurenceCounts[x.Value])
                        .ToList();

                    var enumMemberChildrenWithOccurrences = enumMemberChildren
                        .Where(x => !enumSummary.EnumMembersWithZeroOcurrences.Contains(x.Value))
                        .ToList();

                    enumMemberChildrenWithOccurrences.ForEach(finalizedBricks.Add);
                    var membersWithZeroOcurrences = enumSummary.EnumMembersWithZeroOcurrences.Count;
                    bool allAreOmitted = membersWithZeroOcurrences == enumSummary.EnumTotalMemberCount;

                    if (membersWithZeroOcurrences > 0)
                        finalizedBricks.Add(new RegexBrickOmittedCount(enumNode, membersWithZeroOcurrences, allAreOmitted));
                }
            }
        }

        return finalizedBricks;
    }

    public override string ToString() =>
        string.Join("\r\n", Lines);
}