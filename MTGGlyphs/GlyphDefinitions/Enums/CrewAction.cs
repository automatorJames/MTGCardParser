namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a pilot does with more power: "crews vehicles", "saddles mounts", "saddles mounts and crews vehicles", in "… as though its power were 2 greater".</summary>
public enum CrewAction
{
    [RegexPattern("saddles mounts and crews vehicles")]
    SaddlesAndCrews,

    [RegexPattern("crews vehicles")]
    Crews,

    [RegexPattern("saddles mounts")]
    Saddles
}
