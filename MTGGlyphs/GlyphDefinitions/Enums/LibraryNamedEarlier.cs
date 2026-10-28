namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A library referred back to rather than named by its owner, the determiner in "of that library" or "of the library": "that", "the".</summary>
public enum LibraryNamedEarlier
{
    [RegexPattern("that", "the")]
    That
}
