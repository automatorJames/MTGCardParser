namespace MTGGlyphs.GlyphDefinitions;

public enum CopyTypeAddition
{
    [RegexPattern("in addition to its other types")]
    Types,

    [RegexPattern("in addition to its other colors and types")]
    ColorsAndTypes,

    [RegexPattern("in addition to its other creature types")]
    CreatureTypes
}
