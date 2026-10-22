namespace MTGGlyphs.GlyphDefinitions;

public enum PastPlayerAction
{
    [RegexPattern("didn't cast", "haven't cast", "hasn't cast")]
    DidntCast,

    [RegexPattern("cast", "has cast")]
    Cast,

    [RegexPattern("didn't play", "haven't played")]
    DidntPlay,

    [RegexPattern("played")]
    Played,

    [RegexPattern("gained life")]
    GainedLife,

    [RegexPattern("lost life")]
    LostLife,

    [RegexPattern("searched their library", "searched your library")]
    SearchedLibrary,

    [RegexPattern("sacrificed")]
    Sacrificed
}
