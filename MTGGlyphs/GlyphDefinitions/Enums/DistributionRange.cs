namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How many objects counters are distributed among: "one or two", "one, two, or three", "any number of", in "distribute two +1/+1 counters among one or two target creatures".</summary>
public enum DistributionRange
{
    [RegexPattern("one or two")]
    OneOrTwo,

    [RegexPattern("one, two, or three")]
    OneTwoOrThree,

    [RegexPattern("any number of")]
    AnyNumber
}
