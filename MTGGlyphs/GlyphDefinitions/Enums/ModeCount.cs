namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How many modes of a modal spell or ability are chosen, after "choose": "one", "two", "both", "one or both", "one or more", "up to two", "any number", "up to five {p} worth of modes".</summary>
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

    [RegexPattern("up to that many")]
    UpToThatMany,

    [RegexPattern("up to x")]
    UpToX,

    [RegexPattern(@"up to five \{p\} worth of modes")]
    UpToFivePawprints,

    [RegexPattern("any number")]
    AnyNumber,

    One,

    Two,

    Three,

    Four,

    Both
}
