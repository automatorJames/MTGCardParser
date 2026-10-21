namespace MTGGlyphs.GlyphDefinitions;

public enum PlayVerb
{
    [RegexPattern("play", "plays")]
    Play,

    [RegexPattern("cast", "casts")]
    Cast
}
