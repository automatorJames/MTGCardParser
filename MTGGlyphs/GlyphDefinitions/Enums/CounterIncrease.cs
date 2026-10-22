namespace MTGGlyphs.GlyphDefinitions;

public enum CounterIncrease
{
    [RegexPattern("twice that many")]
    Double,

    [RegexPattern("that many plus one")]
    PlusOne
}
