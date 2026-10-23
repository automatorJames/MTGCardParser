namespace MTGGlyphs.GlyphDefinitions;

public enum TargetPrefix
{
    [RegexPattern("up to one other")]
    UpToOneOther,

    [RegexPattern("up to one")]
    UpToOne,

    Another,

    [RegexPattern("a second")]
    ASecond,

    [RegexPattern("a third")]
    AThird
}
