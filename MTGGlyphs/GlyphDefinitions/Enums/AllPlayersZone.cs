namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A zone taken across every player, as a place cards are in: "cards in graveyards", "creature cards in graveyards and libraries", "all hands".</summary>
public enum AllPlayersZone
{
    [RegexPattern("all graveyards", "graveyards")]
    Graveyards,

    [RegexPattern("all libraries", "libraries")]
    Libraries,

    [RegexPattern("all hands", "hands")]
    Hands
}
