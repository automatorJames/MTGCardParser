namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Words before "target" setting how many or which: "up to one" ("up to one target creature"), "up to one other", "another" ("another target creature"), "a second", or "a third" ("a second target creature", "a third target creature").</summary>
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
