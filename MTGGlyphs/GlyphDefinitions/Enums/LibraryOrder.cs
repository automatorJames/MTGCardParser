namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The order cards go into a library in, after the position: "in any order", "in a random order" ("on the bottom of your library in a random order").</summary>
public enum LibraryOrder
{
    [RegexPattern("in any order")]
    AnyOrder,

    [RegexPattern("in a random order")]
    RandomOrder
}
