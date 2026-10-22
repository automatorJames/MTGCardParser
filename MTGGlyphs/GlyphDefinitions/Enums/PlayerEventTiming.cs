namespace MTGGlyphs.GlyphDefinitions;

public enum PlayerEventTiming
{
    [RegexPattern("for the first time each turn")]
    FirstTimeEachTurn,

    [RegexPattern("for the first time during each of your turns")]
    FirstTimeDuringEachOfYourTurns,

    [RegexPattern("during your turn")]
    DuringYourTurn
}
