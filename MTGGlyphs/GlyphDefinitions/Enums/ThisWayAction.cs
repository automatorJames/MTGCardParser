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
    DealtDamage
}
