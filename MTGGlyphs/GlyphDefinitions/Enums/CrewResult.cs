namespace MTGGlyphs.GlyphDefinitions;

public enum CrewResult
{
    [RegexPattern("this vehicle becomes an artifact creature")]
    Crewed,

    [RegexPattern("this mount becomes saddled")]
    Saddled
}
