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

    [RegexPattern("was dealt damage", "were dealt damage")]
    WasDealtDamage,

    [RegexPattern("wasn't dealt damage", "weren't dealt damage")]
    WasntDealtDamage,

    [RegexPattern("blocked or was blocked by", "blocked or were blocked by")]
    BlockedOrWasBlockedBy,

    [RegexPattern("was blocked by", "were blocked by")]
    WasBlockedBy,

    [RegexPattern("was blocked", "were blocked")]
    WasBlocked,

    [RegexPattern("blocked")]
    Blocked,

    [RegexPattern("regenerated")]
    Regenerated,

    [RegexPattern("dealt damage to an opponent")]
    DealtDamageToAnOpponent,

    [RegexPattern("dealt combat damage to a player")]
    DealtCombatDamageToAPlayer,

    [RegexPattern("didn't deal combat damage to a creature")]
    DidntDealCombatDamageToACreature,

    [RegexPattern("dealt damage to you")]
    DealtDamageToYou,

    [RegexPattern("dealt damage")]
    DealtDamage
}
