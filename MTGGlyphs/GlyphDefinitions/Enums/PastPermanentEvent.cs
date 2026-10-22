namespace MTGGlyphs.GlyphDefinitions;

public enum PastPermanentEvent
{
    [RegexPattern("entered the battlefield", "entered")]
    Entered,

    [RegexPattern("didn't enter the battlefield", "didn't enter")]
    DidntEnter,

    [RegexPattern("died")]
    Died,

    [RegexPattern("left the battlefield")]
    LeftTheBattlefield,

    [RegexPattern("didn't attack")]
    DidntAttack,

    [RegexPattern("attacked")]
    Attacked,

    [RegexPattern("was dealt damage")]
    WasDealtDamage,

    [RegexPattern("was blocked")]
    WasBlocked,

    [RegexPattern("blocked")]
    Blocked,

    [RegexPattern("regenerated")]
    Regenerated,

    [RegexPattern("dealt damage to an opponent")]
    DealtDamageToAnOpponent,

    [RegexPattern("dealt combat damage to a player")]
    DealtCombatDamageToAPlayer,

    [RegexPattern("dealt damage to you")]
    DealtDamageToYou,

    [RegexPattern("dealt damage")]
    DealtDamage
}
