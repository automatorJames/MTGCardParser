namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What the effect just described did to an object, before "this way": "tapped", "sacrificed", "exiled", "revealed", "milled", "dies", "dealt damage", "put into exile", "searches their library", as in "a creature is dealt damage this way", "creature tapped this way", "that creature dies this way".</summary>
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
