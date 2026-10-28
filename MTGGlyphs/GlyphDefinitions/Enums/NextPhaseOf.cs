namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whose next step or phase an effect lasts until, after "until": "your next" ("until your next upkeep"), "its controller's next" ("until its controller's next untap step"), "that player's next".</summary>
public enum NextPhaseOf
{
    [RegexPattern("your next")]
    YourNext,

    [RegexPattern("its controller's next", "their controller's next")]
    ItsControllersNext,

    [RegexPattern("that player's next")]
    ThatPlayersNext
}
