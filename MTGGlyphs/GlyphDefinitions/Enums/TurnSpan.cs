namespace MTGGlyphs.GlyphDefinitions;

public enum TurnSpan
{
    [RegexPattern("this turn")]
    ThisTurn,

    [RegexPattern("last turn")]
    LastTurn,

    [RegexPattern("this combat")]
    ThisCombat,

    [RegexPattern("during your last turn")]
    DuringYourLastTurn,

    [RegexPattern("during its controller's last turn")]
    DuringItsControllersLastTurn
}
