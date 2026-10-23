namespace MTGGlyphs.GlyphDefinitions;

public enum ScaleFactor
{
    Half,

    Twice,

    [RegexPattern("a third of")]
    AThird,

    [RegexPattern("three times")]
    ThreeTimes
}
