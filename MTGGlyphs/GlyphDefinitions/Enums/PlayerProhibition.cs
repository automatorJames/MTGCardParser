namespace MTGGlyphs.GlyphDefinitions;

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
