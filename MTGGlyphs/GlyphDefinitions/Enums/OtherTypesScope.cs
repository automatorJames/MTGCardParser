namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Which characteristics an object keeps when it gains a type, after "in addition to its other": "types", "creature types", "land types", "colors", "colors and types" ("it's a zombie in addition to its other creature types").</summary>
public enum OtherTypesScope
{
    [RegexPattern("colors and types")]
    ColorsAndTypes,

    [RegexPattern("creature types")]
    CreatureTypes,

    [RegexPattern("land types")]
    LandTypes,

    Types,

    Colors
}
