namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A stock reminder sentence with nothing variable but which rule it is: "zero is even", "piles can be empty", "round up each time", "they're affected by summoning sickness", "you can't look at it", "do this before you draw", "this effect doesn't end", "that spell won't resolve".</summary>
public enum ReminderRule
{
    [RegexPattern("zero is even")]
    ZeroIsEven,

    [RegexPattern("they're (?:still )?affected by summoning sickness")]
    AffectedBySummoningSickness,

    [RegexPattern("you can't look at it")]
    CantLookAtIt,

    [RegexPattern("do this before you draw")]
    DoThisBeforeYouDraw,

    [RegexPattern("piles can be empty")]
    PilesCanBeEmpty,

    [RegexPattern("round up each time")]
    RoundUpEachTime,

    [RegexPattern("round down each time")]
    RoundDownEachTime,

    [RegexPattern("it's every creature type")]
    EveryCreatureType,

    [RegexPattern("this effect doesn't end")]
    EffectDoesntEnd,

    [RegexPattern("the spell has no effect")]
    SpellHasNoEffect,

    [RegexPattern("that spell won't resolve")]
    SpellWontResolve,

    [RegexPattern("it can attack this turn")]
    CanAttackThisTurn,

    [RegexPattern("count each ability only once")]
    CountEachAbilityOnce
}
