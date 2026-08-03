namespace MTGPlexer.RegexGeneration.Graph.Bricks.PostFormatting;

public class RegexBrickSynonymSectionFooter : RegexBrick
{
    public const char DividerChar = '─'; // ─

    public RegexBrickSynonymSectionFooter(RegexNode parentNode, int boxInteriorLength)
        : base(parentNode, null, GetComment(boxInteriorLength))
    {
    }

    static string GetComment(int boxInteriorLength) =>
        new string(DividerChar, boxInteriorLength);
}
