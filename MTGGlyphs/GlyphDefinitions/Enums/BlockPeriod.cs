namespace MTGGlyphs.GlyphDefinitions;

/// <summary>For how long a block permission holds: "can block an additional creature each combat", "can block any number of creatures this turn".</summary>
public enum BlockPeriod
{
    [RegexPattern("each combat")]
    EachCombat,

    [RegexPattern("this turn")]
    ThisTurn
}
