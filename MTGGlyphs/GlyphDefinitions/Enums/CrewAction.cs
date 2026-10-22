namespace MTGGlyphs.GlyphDefinitions;

public enum CrewAction
{
    [RegexPattern("saddles mounts and crews vehicles")]
    SaddlesAndCrews,

    [RegexPattern("crews vehicles")]
    Crews,

    [RegexPattern("saddles mounts")]
    Saddles
}
