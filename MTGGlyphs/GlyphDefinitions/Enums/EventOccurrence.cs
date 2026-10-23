namespace MTGGlyphs.GlyphDefinitions;

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
