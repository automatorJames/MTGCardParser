namespace MTGGlyphs.GlyphDefinitions;

public enum AmountFactor
{
    [RegexPattern("twice", "double")]
    Twice,

    [RegexPattern("triple", "three times")]
    Thrice
}
