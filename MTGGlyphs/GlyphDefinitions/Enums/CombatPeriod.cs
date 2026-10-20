namespace MTGGlyphs.GlyphDefinitions;

public enum CombatPeriod
{
    [RegexPattern("each combat")]
    EachCombat,

    [RegexPattern("this turn")]
    ThisTurn,

    [RegexPattern("this combat")]
    ThisCombat
}
