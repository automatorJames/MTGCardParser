namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>A synthetic brick drawing a divider line under a group of enum-member synonym rows.</summary>
public class RegexBrickSynonymSectionFooter : RegexBrick
{
    public const char DividerChar = '─'; // ─

    public RegexBrickSynonymSectionFooter(RegexNode parentNode, int boxInteriorLength)
        : base(parentNode, null)
    {
        CommentFormatted = GetComment(boxInteriorLength);
    }

    static string GetComment(int boxInteriorLength) =>
        new string(DividerChar, boxInteriorLength);
}
