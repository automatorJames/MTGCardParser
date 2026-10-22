namespace MTGGlyphs.GlyphDefinitions;

public enum PaymentRepetition
{
    [RegexPattern("any number of times")]
    AnyNumberOfTimes,

    [RegexPattern("up to two times")]
    UpToTwoTimes,

    [RegexPattern("up to three times")]
    UpToThreeTimes
}
