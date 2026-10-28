namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The day/night designation: "day", "night", "neither day nor night", in "it becomes night", "it's neither day nor night".</summary>
public enum DayNightState
{
    Day,

    Night,

    [RegexPattern("neither day nor night")]
    Neither
}
