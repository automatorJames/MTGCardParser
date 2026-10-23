namespace MTGGlyphs.GlyphDefinitions;

public enum RestFate
{
    [RegexPattern("discards?")]
    Discard,

    [RegexPattern("exiles?")]
    Exile
}
