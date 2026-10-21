namespace MTGGlyphs.GlyphDefinitions;

public enum ModeCount
{
    [RegexPattern("one or both")]
    OneOrBoth,

    [RegexPattern("one or more")]
    OneOrMore,

    [RegexPattern("up to one")]
    UpToOne,

    [RegexPattern("up to two")]
    UpToTwo,

    [RegexPattern("up to four")]
    UpToFour,

    [RegexPattern("any number")]
    AnyNumber,

    One,

    Two,

    Three,

    Four,

    Both
}
