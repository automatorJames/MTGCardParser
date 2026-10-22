namespace MTGGlyphs.GlyphDefinitions;

public enum SearchFollowUp
{
    [RegexPattern("and reveals? (?:it|them)")]
    Reveal,

    [RegexPattern("and exiles? (?:it|them)")]
    Exile
}
