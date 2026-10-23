namespace MTGGlyphs.GlyphDefinitions;

public enum ThisWayAction
{
    Tapped,

    Sacrificed,

    Exiled,

    Discarded,

    Revealed,

    Destroyed,

    Milled,

    Returned,

    Countered,

    Prevented,

    Chosen,

    [RegexPattern("dies", "died")]
    Dies,

    [RegexPattern("dealt damage")]
    DealtDamage,

    [RegexPattern("regenerates")]
    Regenerates,

    [RegexPattern("connives")]
    Connives,

    [RegexPattern("enters")]
    Enters,

    [RegexPattern("search(?:es)? (?:your|their) library")]
    SearchesLibrary,

    [RegexPattern("put into exile")]
    PutIntoExile,

    [RegexPattern("put into (?:a|your|their|that) graveyard")]
    PutIntoAGraveyard
}
