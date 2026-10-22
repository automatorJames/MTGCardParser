namespace MTGGlyphs.GlyphDefinitions;

public enum PastPermanentEvent
{
    [RegexPattern("entered the battlefield", "entered")]
    Entered,

    [RegexPattern("died")]
    Died,

    [RegexPattern("left the battlefield")]
    LeftTheBattlefield,

    [RegexPattern("didn't attack")]
    DidntAttack,

    [RegexPattern("attacked")]
    Attacked,

    [RegexPattern("was blocked")]
    WasBlocked,

    [RegexPattern("blocked")]
    Blocked,

    [RegexPattern("dealt damage to an opponent")]
    DealtDamageToAnOpponent,

    [RegexPattern("dealt combat damage to a player")]
    DealtCombatDamageToAPlayer,

    [RegexPattern("dealt damage to you")]
    DealtDamageToYou,

    [RegexPattern("dealt damage")]
    DealtDamage
}
