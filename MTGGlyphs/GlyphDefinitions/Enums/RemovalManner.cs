namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How a destroy or exile effect removes its object, trailing the action: "face down" ("exile a card from your hand face down"), "face up", or "at random" ("destroy one of them at random").</summary>
public enum RemovalManner
{
    [RegexPattern("face down")]
    FaceDown,

    [RegexPattern("face up")]
    FaceUp,

    [RegexPattern("at random")]
    AtRandom
}
