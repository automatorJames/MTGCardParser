namespace MTGGlyphs.GlyphDefinitions;

public enum PermanentEvent
{
    [RegexPattern("dies", "die")]
    Dies,

    [RegexPattern("enters", "enter")]
    EntersTheBattlefield,

    LeavesTheBattlefield,

    IsPutIntoAGraveyardFromTheBattlefield,

    IsPutIntoAGraveyardFromAnywhere,

    IsTurnedFaceUp,

    BlocksOrBecomesBlocked,

    [RegexPattern("attacks and isn't blocked")]
    AttacksAndIsntBlocked,

    AttacksOrBlocks,

    [RegexPattern("attacks you or a planeswalker you control")]
    AttacksYouOrAPlaneswalkerYouControl,

    [RegexPattern("attacks you")]
    AttacksYou,

    [RegexPattern("attacks", "attack")]
    Attacks,

    [RegexPattern("blocks", "block")]
    Blocks,

    BecomesBlocked,

    BecomesTapped,

    BecomesUntapped,

    BecomesMonstrous,

    [RegexPattern("mutates")]
    Mutates,

    [RegexPattern("is tapped for mana")]
    IsTappedForMana,

    IsDealtCombatDamage,

    IsDealtDamage,

    [RegexPattern("deals combat damage to a player or planeswalker", "deal combat damage to a player or planeswalker")]
    DealsCombatDamageToAPlayerOrPlaneswalker,

    [RegexPattern("deals combat damage to a player", "deal combat damage to a player")]
    DealsCombatDamageToAPlayer,

    [RegexPattern("deals combat damage to an opponent", "deal combat damage to an opponent")]
    DealsCombatDamageToAnOpponent,

    [RegexPattern("deals combat damage to you")]
    DealsCombatDamageToYou,

    [RegexPattern("deals combat damage to a creature")]
    DealsCombatDamageToACreature,

    [RegexPattern("deals combat damage", "deal combat damage")]
    DealsCombatDamage,

    [RegexPattern("deals damage", "deal damage")]
    DealsDamage
}
