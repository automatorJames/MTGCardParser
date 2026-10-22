namespace MTGGlyphs.GlyphDefinitions;

public enum RepeatCount
{
    Once,

    [RegexPattern("any number of times")]
    AnyNumberOfTimes,

    [RegexPattern("as many times as (they|you) choose")]
    AsManyTimesAsChosen
}
