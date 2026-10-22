namespace MTGGlyphs.GlyphDefinitions;

public enum PlayerResponse
{
    Does,

    [RegexPattern("doesn't")]
    Doesnt,

    [RegexPattern("can't")]
    Cant
}
