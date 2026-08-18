namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>A synthetic brick summarizing enum members that never occurred in the analyzed data, e.g. "3 omitted".</summary>
public class RegexBrickOmittedCount : RegexBrick
{
    public RegexBrickOmittedCount(RegexNode parentNode, int omittedCount, bool allAreOmitted)
        : base(parentNode, null)
    {
        CommentFormatted = GetComment(omittedCount, allAreOmitted);
    }

    static string GetComment(int omittedCount, bool allAreOmitted)
    {
        var comment = $"{omittedCount} omitted";

        if (allAreOmitted)
            comment = "All " + comment;

        return comment;
    }
}