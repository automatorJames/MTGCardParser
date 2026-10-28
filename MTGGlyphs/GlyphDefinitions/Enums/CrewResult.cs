namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What crew and saddle reminder text says happens: "this vehicle becomes an artifact creature" (until end of turn), "this mount becomes saddled".</summary>
public enum CrewResult
{
    [RegexPattern("this vehicle becomes an artifact creature")]
    Crewed,

    [RegexPattern("this mount becomes saddled")]
    Saddled
}
