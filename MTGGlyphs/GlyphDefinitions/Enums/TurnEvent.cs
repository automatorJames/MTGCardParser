namespace MTGGlyphs.GlyphDefinitions;

public enum TurnEvent
{
    [RegexPattern("you attacked", "you've attacked", "you attacked with a creature")]
    YouAttacked,

    [RegexPattern("you didn't attack with a creature", "you didn't attack")]
    YouDidntAttack,

    [RegexPattern("a creature died")]
    CreatureDied,

    [RegexPattern("a permanent left the battlefield under your control", "a permanent you controlled left the battlefield")]
    PermanentLeft,

    [RegexPattern("an opponent lost life")]
    OpponentLostLife,

    [RegexPattern("you gained life", "you've gained life")]
    YouGainedLife,

    [RegexPattern("you lost life")]
    YouLostLife,

    [RegexPattern("you didn't lose life")]
    YouDidntLoseLife,

    [RegexPattern("you gained or lost life")]
    YouGainedOrLostLife,

    [RegexPattern("you've cast another spell", "you cast another spell")]
    CastAnotherSpell,

    [RegexPattern("you didn't play a land")]
    YouDidntPlayLand,

    [RegexPattern("you descended")]
    YouDescended,

    [RegexPattern("you've committed a crime", "you committed a crime")]
    YouCommittedCrime,

    [RegexPattern("you've scried or surveilled", "you scried or surveilled")]
    YouScriedOrSurveilled,

    [RegexPattern("a spell was warped")]
    SpellWasWarped
}
