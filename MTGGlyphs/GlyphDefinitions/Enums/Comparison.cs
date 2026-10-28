namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The direction of a bound: "less" or "greater" (also "more", "lesser"), as in "with power 2 or less", "4 or greater", "less than or equal to the number of cards in your graveyard", "with lesser mana value".</summary>
public enum Comparison
{
    [RegexPattern("lesser", "less")]
    Less,

    [RegexPattern("greater", "more")]
    Greater
}
