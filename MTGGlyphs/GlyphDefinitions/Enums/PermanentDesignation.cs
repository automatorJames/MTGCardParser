namespace MTGGlyphs.GlyphDefinitions;

public enum PermanentDesignation
{
    [RegexPattern("face-down")]
    FaceDown,

    [RegexPattern("face-up")]
    FaceUp,

    Suspected,

    Modified,

    Goaded,

    Saddled,

    Monstrous,

    Renowned,

    Equipped
}
