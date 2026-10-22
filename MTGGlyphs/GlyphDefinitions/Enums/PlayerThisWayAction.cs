namespace MTGGlyphs.GlyphDefinitions;

public enum PlayerThisWayAction
{
    [RegexPattern("discards?", "discarded")]
    Discard,

    [RegexPattern("sacrifices?", "sacrificed")]
    Sacrifice,

    [RegexPattern("exiles?", "exiled")]
    Exile,

    [RegexPattern("cast")]
    Cast,

    [RegexPattern("mills?", "milled")]
    Mill
}
