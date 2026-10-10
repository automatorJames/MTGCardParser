namespace MTGGlyphs.GlyphDefinitions;

public enum RemovalVerb
{
    [RegexPattern("destroys?")]
    Destroy,

    [RegexPattern("exiles?")]
    Exile
}
