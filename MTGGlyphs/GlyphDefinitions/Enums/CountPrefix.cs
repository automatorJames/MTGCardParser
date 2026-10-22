namespace MTGGlyphs.GlyphDefinitions;

public enum CountPrefix
{
    [RegexPattern("up to")]
    UpTo,

    Exactly,

    [RegexPattern("fewer than")]
    FewerThan,

    [RegexPattern("at least")]
    AtLeast
}
