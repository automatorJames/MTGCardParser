namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The zone or zones a search goes through, after "search your" or "search target player's": plain "library", "library and/or graveyard", "hand and/or library", "library and/or outside the game", or "graveyard, hand, and library" (the extirpate effects).</summary>
public enum SearchedZones
{
    [RegexPattern("graveyard, hand, and library", "library, hand, and graveyard")]
    GraveyardHandAndLibrary,

    [RegexPattern("graveyard, hand, and/or library", "library, hand, and/or graveyard", "hand, graveyard, and/or library")]
    GraveyardHandAndOrLibrary,

    [RegexPattern("library, graveyard, and/or outside the game")]
    LibraryGraveyardAndOrOutsideTheGame,

    [RegexPattern("library and/or outside the game")]
    LibraryAndOrOutsideTheGame,

    [RegexPattern("library and/or graveyard", "graveyard and/or library")]
    LibraryAndOrGraveyard,

    [RegexPattern("hand and/or library", "library and/or hand")]
    HandAndOrLibrary,

    [RegexPattern("library and graveyard", "graveyard and library")]
    LibraryAndGraveyard,

    Library
}
