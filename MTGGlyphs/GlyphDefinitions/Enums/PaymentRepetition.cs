namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How many times a cost may be paid, after "pay" and the cost: "any number of times", "up to two times", "up to three times" ("pay {1}{w} any number of times").</summary>
public enum PaymentRepetition
{
    [RegexPattern("any number of times")]
    AnyNumberOfTimes,

    [RegexPattern("up to two times")]
    UpToTwoTimes,

    [RegexPattern("up to three times")]
    UpToThreeTimes
}
