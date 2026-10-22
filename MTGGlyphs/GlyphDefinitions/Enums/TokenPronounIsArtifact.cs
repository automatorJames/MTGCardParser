namespace MTGGlyphs.GlyphDefinitions;

public enum TokenPronounIsArtifact
{
    [RegexPattern("it's an artifact with \"")]
    It,

    [RegexPattern("they're artifacts with \"")]
    They
}
