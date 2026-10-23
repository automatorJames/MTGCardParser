namespace MTGGlyphs.GlyphDefinitions;

public enum RemovalManner
{
    [RegexPattern("face down")]
    FaceDown,

    [RegexPattern("face up")]
    FaceUp,

    [RegexPattern("at random")]
    AtRandom
}
