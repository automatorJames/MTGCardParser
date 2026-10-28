namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The span a condition or count looks back over: "this turn" ("you attacked this turn", "cards you've drawn this turn"), "last turn" ("an opponent lost life last turn"), "this combat", "during your last turn", or "during its controller's last turn".</summary>
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
