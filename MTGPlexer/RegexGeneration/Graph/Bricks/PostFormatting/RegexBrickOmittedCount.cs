namespace MTGPlexer.RegexGeneration.Graph.Bricks.PostFormatting;

public class RegexBrickOmittedCount : RegexBrick
{
    public RegexBrickOmittedCount(RegexNode parentNode, int omittedCount, bool allAreOmitted)
        : base(parentNode, null, GetComment(omittedCount, allAreOmitted))
    {
    }

    static string GetComment(int omittedCount, bool allAreOmitted)
    {
        var comment = $"{omittedCount} omitted";

        if (allAreOmitted)
            comment = "All " + comment;

        return comment;
    }
}