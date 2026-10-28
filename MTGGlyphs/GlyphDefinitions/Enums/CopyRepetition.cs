namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How many times something is done, as copying, activation caps and repeated keyword actions count it: "twice", "three times", "x times", "an additional time" - "copy it three times", "activate no more than twice each turn", "investigate three times".</summary>
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
