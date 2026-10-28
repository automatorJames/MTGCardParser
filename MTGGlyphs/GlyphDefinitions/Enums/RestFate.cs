namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What becomes of the cards not chosen from a revealed or looked-at set, before "the rest": "discards the rest" or "exile the rest".</summary>
public enum RestFate
{
    [RegexPattern("discards?")]
    Discard,

    [RegexPattern("exiles?")]
    Exile
}
