namespace MTGGlyphs.GlyphDefinitions;

public enum AnyTargetChoice
{
    Any,

    [RegexPattern("any other")]
    AnyOther,

    Another,

    [RegexPattern("a second")]
    ASecond,

    [RegexPattern("a third")]
    AThird
}
