namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A game action a player can be barred from, after "can't": "draw cards", "draw more than one card each turn", "search libraries", "get counters", "get poison counters", "cycle cards".</summary>
public enum PlayerProhibition
{
    [RegexPattern("draw cards")]
    DrawCards,

    [RegexPattern("draw more than one card each turn")]
    DrawMoreThanOne,

    [RegexPattern("search libraries")]
    SearchLibraries,

    [RegexPattern("get counters")]
    GetCounters,

    [RegexPattern("get poison counters")]
    GetPoisonCounters,

    [RegexPattern("cycle cards")]
    CycleCards
}
