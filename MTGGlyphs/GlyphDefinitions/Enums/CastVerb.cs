namespace MTGGlyphs.GlyphDefinitions;

public enum CastVerb
{
    [RegexPattern("cast or copy", "casts or copies")]
    CastOrCopy,

    [RegexPattern("casts?")]
    Cast
}
