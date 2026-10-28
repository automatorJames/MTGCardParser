namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a permanent does at the moment a replacement effect applies, after the permanent: "{this} enters", "this creature attacks", "{this} enters or is turned face up", "is turned face up".</summary>
public enum AsPermanentEvent
{
    Enters,

    Attacks,

    [RegexPattern("enters or is turned face up")]
    EntersOrIsTurnedFaceUp,

    [RegexPattern("is turned face up")]
    IsTurnedFaceUp
}
