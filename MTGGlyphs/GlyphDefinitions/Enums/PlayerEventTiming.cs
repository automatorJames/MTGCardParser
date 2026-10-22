namespace MTGGlyphs.GlyphDefinitions;

public enum PlayerEventTiming
{
    [RegexPattern("for the first time each turn")]
    FirstTimeEachTurn,

    [RegexPattern("during your turn")]
    DuringYourTurn
}
