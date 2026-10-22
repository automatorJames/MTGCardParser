namespace MTGGlyphs.GlyphDefinitions;

public enum EntryState
{
    [RegexPattern("tapped and attacking")]
    TappedAndAttacking,

    Tapped,

    Untapped,

    [RegexPattern("face down")]
    FaceDown
}
