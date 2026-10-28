namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What precedes "excess damage" when it's taken as an amount: "the amount of", "that amount of", "that", "the" - "that excess damage", "the amount of excess damage dealt to that creature this way".</summary>
public enum ExcessMeasure
{
    [RegexPattern("the amount of")]
    AmountOf,

    [RegexPattern("that amount of")]
    ThatAmountOf,

    That,

    The
}
