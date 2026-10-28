namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How many times a repeated process is run: "once" ("repeat this process once"), "any number of times" ("you may repeat this process any number of times"), or "as many times as you choose".</summary>
public enum RepeatCount
{
    Once,

    [RegexPattern("any number of times")]
    AnyNumberOfTimes,

    [RegexPattern("as many times as (they|you) choose")]
    AsManyTimesAsChosen
}
