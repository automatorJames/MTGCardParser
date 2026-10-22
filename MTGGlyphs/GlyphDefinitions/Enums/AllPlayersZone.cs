namespace MTGGlyphs.GlyphDefinitions;

public enum AllPlayersZone
{
    [RegexPattern("all graveyards", "graveyards")]
    Graveyards,

    [RegexPattern("all libraries", "libraries")]
    Libraries,

    [RegexPattern("all hands", "hands")]
    Hands
}
