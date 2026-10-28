namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a player did to an object as the effect just described let them, before "this way", present or past: "discard" ("discards", "discarded"), "sacrifice", "exile", "cast", "mill", "reveal" ("you discard a creature card this way", "you exiled a land card this way").</summary>
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
    Mill,

    [RegexPattern("reveals?", "revealed")]
    Reveal
}
