namespace MTGGlyphs.GlyphDefinitions;

public enum DistributionRange
{
    [RegexPattern("one or two")]
    OneOrTwo,

    [RegexPattern("one, two, or three")]
    OneTwoOrThree,

    [RegexPattern("any number of")]
    AnyNumber
}
