namespace MTGGlyphs.GlyphDefinitions;

public enum NextPhaseOf
{
    [RegexPattern("your next")]
    YourNext,

    [RegexPattern("its controller's next", "their controller's next")]
    ItsControllersNext,

    [RegexPattern("that player's next")]
    ThatPlayersNext
}
