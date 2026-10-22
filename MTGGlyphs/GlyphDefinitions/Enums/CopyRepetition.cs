namespace MTGGlyphs.GlyphDefinitions;

public enum CopyRepetition
{
    Twice,

    [RegexPattern("three times")]
    ThreeTimes,

    [RegexPattern("x times")]
    XTimes,

    [RegexPattern("an additional time")]
    AnAdditionalTime
}
