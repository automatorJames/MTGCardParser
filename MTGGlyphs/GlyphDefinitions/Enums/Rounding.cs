namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How a halved or divided amount is rounded, "rounded down" or "rounded up", trailing the fraction: "half your life, rounded up", "half the number of zombies you control, rounded down".</summary>
public enum Rounding
{
    [RegexPattern("rounded down")]
    Down,

    [RegexPattern("rounded up")]
    Up
}
