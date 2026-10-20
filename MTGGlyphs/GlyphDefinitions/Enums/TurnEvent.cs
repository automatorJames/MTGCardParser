namespace MTGGlyphs.GlyphDefinitions;

public enum TurnEvent
{
    [RegexPattern("you attacked", "you've attacked", "you attacked with a creature")]
    YouAttacked,

    [RegexPattern("a creature died")]
    CreatureDied,

    [RegexPattern("a permanent left the battlefield under your control", "a permanent you controlled left the battlefield")]
    PermanentLeft,

    [RegexPattern("an opponent lost life")]
    OpponentLostLife,

    [RegexPattern("you gained life", "you've gained life")]
    YouGainedLife,

    [RegexPattern("you gained or lost life")]
    YouGainedOrLostLife,

    [RegexPattern("you've cast another spell", "you cast another spell")]
    CastAnotherSpell,

    [RegexPattern("you descended")]
    YouDescended
}
