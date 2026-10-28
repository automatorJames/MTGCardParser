namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How often or when a player event counts for a trigger: "for the first time each turn", "for the first time during each of your turns", "during your turn" ("whenever you gain life for the first time each turn, ...").</summary>
public enum PlayerEventTiming
{
    [RegexPattern("for the first time each turn")]
    FirstTimeEachTurn,

    [RegexPattern("for the first time during each of your turns")]
    FirstTimeDuringEachOfYourTurns,

    [RegexPattern("during your turn")]
    DuringYourTurn
}
