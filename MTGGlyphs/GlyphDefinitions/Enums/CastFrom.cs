namespace MTGGlyphs.GlyphDefinitions;

public enum CastFrom
{
    [RegexPattern("from your hand")]
    Hand,

    [RegexPattern("from exile")]
    Exile,

    [RegexPattern("from your graveyard")]
    Graveyard,

    [RegexPattern("from graveyards", "from their graveyards")]
    Graveyards,

    [RegexPattern("from graveyards or libraries")]
    GraveyardsOrLibraries,

    [RegexPattern("from graveyards or exile")]
    GraveyardsOrExile,

    [RegexPattern("from anywhere other than their hands?", "from anywhere other than your hand")]
    AnywhereButHand
}
