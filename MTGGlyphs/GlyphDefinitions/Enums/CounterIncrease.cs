namespace MTGGlyphs.GlyphDefinitions;

public enum CounterIncrease
{
    [RegexPattern("twice that many")]
    Double,

    [RegexPattern("three times that many")]
    Triple,

    [RegexPattern("that many plus one")]
    PlusOne,

    [RegexPattern("half that many")]
    Half
}
