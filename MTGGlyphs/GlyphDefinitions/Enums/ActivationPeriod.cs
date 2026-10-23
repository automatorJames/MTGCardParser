namespace MTGGlyphs.GlyphDefinitions;

public enum ActivationPeriod
{
    [RegexPattern("each turn")]
    EachTurn,

    [RegexPattern("this turn")]
    ThisTurn
}
