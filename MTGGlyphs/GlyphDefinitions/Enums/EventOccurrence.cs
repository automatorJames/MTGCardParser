namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A limit on when a permanent trigger fires: "for the first time each turn", "during your turn", "during an opponent's turn", "during combat" - "whenever a creature enters under your control during your turn, …".</summary>
public enum EventOccurrence
{
    [RegexPattern("for the first time each turn")]
    FirstTimeEachTurn,

    [RegexPattern("during your turn")]
    DuringYourTurn,

    [RegexPattern("during an opponent's turn")]
    DuringAnOpponentsTurn,

    [RegexPattern("during combat")]
    DuringCombat
}
