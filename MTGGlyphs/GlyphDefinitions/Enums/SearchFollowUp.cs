namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What's done with the cards a search finds, tacked onto the search sentence: "and reveal them" ("search your library for up to two creature cards and reveal them") or "and exile it".</summary>
public enum SearchFollowUp
{
    [RegexPattern("and reveals? (?:it|them)")]
    Reveal,

    [RegexPattern("and exiles? (?:it|them)")]
    Exile
}
