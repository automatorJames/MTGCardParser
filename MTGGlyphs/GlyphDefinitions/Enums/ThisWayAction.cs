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
    SearchesLibrary
}
