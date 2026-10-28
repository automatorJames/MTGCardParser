namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A basic land type ("plains", "island", "swamp", "mountain", "forest") or "town", as a kind of land or what a land becomes: "an island", "swamps", "towns".</summary>
public enum LandType
{
    Plains,

    Island,

    Swamp,

    Mountain,

    Forest,

    [RegexPattern("towns?")]
    Town
}
