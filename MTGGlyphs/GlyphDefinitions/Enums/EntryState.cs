namespace MTGGlyphs.GlyphDefinitions;

public enum EntryState
{
    [RegexPattern("tapped and attacking")]
    TappedAndAttacking,

    Tapped,

    [RegexPattern("face down")]
    FaceDown
}
