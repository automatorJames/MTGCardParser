namespace MTGGlyphs.GlyphDefinitions;

public enum PermanentEvent
{
    Dies,

    [RegexPattern("enters")]
    EntersTheBattlefield,

    LeavesTheBattlefield,

    IsPutIntoAGraveyardFromTheBattlefield,

    BlocksOrBecomesBlocked,

    [RegexPattern("attacks and isn't blocked")]
    AttacksAndIsntBlocked,

    AttacksOrBlocks,

    Attacks,

    Blocks,

    BecomesBlocked,

    BecomesTapped,

    IsDealtDamage,

    [RegexPattern("deals combat damage to a player or planeswalker")]
    DealsCombatDamageToAPlayerOrPlaneswalker,

    [RegexPattern("deals combat damage to a player")]
    DealsCombatDamageToAPlayer,

    [RegexPattern("deals combat damage to an opponent")]
    DealsCombatDamageToAnOpponent,

    [RegexPattern("deals combat damage to a creature")]
    DealsCombatDamageToACreature,

    DealsDamage
}
