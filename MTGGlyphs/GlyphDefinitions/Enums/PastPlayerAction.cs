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
    Sacrificed,

    [RegexPattern("discarded")]
    Discarded,

    [RegexPattern("was dealt damage", "were dealt damage", "has been dealt damage")]
    WasDealtDamage,

    [RegexPattern("was dealt combat damage", "were dealt combat damage", "has been dealt combat damage")]
    WasDealtCombatDamage,

    [RegexPattern("was dealt noncombat damage", "were dealt noncombat damage", "has been dealt noncombat damage")]
    WasDealtNoncombatDamage,

    [RegexPattern("attacked with")]
    AttackedWith,

    [RegexPattern("attacked")]
    Attacked
}
