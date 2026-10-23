namespace MTGGlyphs.GlyphDefinitions;

public enum AsPermanentEvent
{
    Enters,

    Attacks,

    [RegexPattern("enters or is turned face up")]
    EntersOrIsTurnedFaceUp,

    [RegexPattern("is turned face up")]
    IsTurnedFaceUp
}
