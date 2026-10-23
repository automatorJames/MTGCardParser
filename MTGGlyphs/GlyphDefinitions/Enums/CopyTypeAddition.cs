namespace MTGGlyphs.GlyphDefinitions;

public enum CopyTypeAddition
{
    [RegexPattern("in addition to (its|his|her) other types")]
    Types,

    [RegexPattern("in addition to (its|his|her) other colors and types")]
    ColorsAndTypes,

    [RegexPattern("in addition to (its|his|her) other creature types")]
    CreatureTypes
}
