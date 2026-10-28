namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The zone a spell is cast (or a card played) from: "from your hand", "from exile", "from your graveyard or from exile", "from graveyards", "from anywhere other than their hands", as in "cast that card from exile", "spells you cast from your graveyard".</summary>
public enum CastFrom
{
    [RegexPattern("from your hand")]
    Hand,

    [RegexPattern("from exile")]
    Exile,

    [RegexPattern("from your graveyard")]
    Graveyard,

    [RegexPattern("from your graveyard or from exile")]
    GraveyardOrExile,

    [RegexPattern("from graveyards", "from their graveyards")]
    Graveyards,

    [RegexPattern("from graveyards or libraries")]
    GraveyardsOrLibraries,

    [RegexPattern("from graveyards or exile", "from graveyards or from exile")]
    GraveyardsOrExile,

    [RegexPattern("from anywhere other than their hands?", "from anywhere other than your hand")]
    AnywhereButHand
}
