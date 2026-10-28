namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The clause keeping a copy's (or changed object's) existing characteristics alongside new ones: "in addition to its other types", "in addition to its other colors and types", "in addition to its other creature types" (also "his"/"her" for named characters).</summary>
public enum CopyTypeAddition
{
    [RegexPattern("in addition to (its|his|her) other types")]
    Types,

    [RegexPattern("in addition to (its|his|her) other colors and types")]
    ColorsAndTypes,

    [RegexPattern("in addition to (its|his|her) other creature types")]
    CreatureTypes
}
