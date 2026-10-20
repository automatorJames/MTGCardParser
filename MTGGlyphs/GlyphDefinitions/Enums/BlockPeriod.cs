namespace MTGGlyphs.GlyphDefinitions;

public enum BlockPeriod
{
    [RegexPattern("each combat")]
    EachCombat,

    [RegexPattern("this turn")]
    ThisTurn
}
