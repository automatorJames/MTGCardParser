namespace MTGGlyphs.GlyphDefinitions;

public enum CountBound
{
    [RegexPattern("more")]
    AtLeast,

    [RegexPattern("fewer")]
    AtMost
}
