namespace MTGGlyphs.GlyphDefinitions;

public enum SearchedZones
{
    [RegexPattern("graveyard, hand, and library")]
    GraveyardHandAndLibrary,

    [RegexPattern("graveyard, hand, and/or library")]
    GraveyardHandAndOrLibrary,

    [RegexPattern("library and/or graveyard", "graveyard and/or library")]
    LibraryAndOrGraveyard,

    [RegexPattern("library and graveyard", "graveyard and library")]
    LibraryAndGraveyard,

    Library
}
