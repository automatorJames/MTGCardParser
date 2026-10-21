namespace MTGGlyphs.GlyphDefinitions;

public enum TokenEntryState
{
    [RegexPattern("tapped and attacking")]
    TappedAndAttacking,

    [RegexPattern("tapped")]
    Tapped
}
