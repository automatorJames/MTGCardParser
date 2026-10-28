namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Which way a permanent is turned: "face up" or "face down", in "turn target face-down creature face up", "turn this creature face down".</summary>
public enum FaceDirection
{
    [RegexPattern("face up")]
    FaceUp,

    [RegexPattern("face down")]
    FaceDown
}
