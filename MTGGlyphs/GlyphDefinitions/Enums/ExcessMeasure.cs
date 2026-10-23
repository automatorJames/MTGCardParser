namespace MTGGlyphs.GlyphDefinitions;

public enum ExcessMeasure
{
    [RegexPattern("the amount of")]
    AmountOf,

    [RegexPattern("that amount of")]
    ThatAmountOf,

    That,

    The
}
