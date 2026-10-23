namespace MTGGlyphs.GlyphDefinitions;

public enum PermanentEvent
{
    [RegexPattern("dies", "die")]
    Dies,

    [RegexPattern("enters untapped")]
    EntersUntapped,

    [RegexPattern("enters", "enter")]
    EntersTheBattlefield,

    LeavesTheBattlefield,

    IsPutIntoAGraveyardFromTheBattlefield,

    IsPutIntoAGraveyardFromAnywhere,

    IsPutIntoAGraveyard,

    IsPutIntoExileFromTheBattlefield,

    IsTurnedFaceUp,

    BlocksOrBecomesBlocked,

    [RegexPattern("attacks and isn't blocked")]
    AttacksAndIsntBlocked,

    AttacksOrBlocks,

    [RegexPattern("attacks you or a planeswalker you control")]
    AttacksYouOrAPlaneswalkerYouControl,

    [RegexPattern("attacks you", "attack you")]
    AttacksYou,

    [RegexPattern("attacks a player", "attack a player")]
    AttacksAPlayer,

    [RegexPattern("attacks alone", "attack alone")]
    AttacksAlone,

    [RegexPattern("attacks", "attack")]
    Attacks,

    [RegexPattern("blocks two or more creatures")]
    BlocksTwoOrMoreCreatures,

    [RegexPattern("blocks a creature")]
    BlocksACreature,

    [RegexPattern("blocks", "block")]
    Blocks,

    BecomesBlocked,

    [RegexPattern("fights", "fight")]
    Fights,

    [RegexPattern("becomes tapped", "become tapped")]
    BecomesTapped,

    BecomesUntapped,

    BecomesMonstrous,

    [RegexPattern("becomes renowned")]
    BecomesRenowned,

    [RegexPattern("mutates")]
    Mutates,

    [RegexPattern("evolves")]
    Evolves,

    [RegexPattern("exploits a creature")]
    ExploitsACreature,

    [RegexPattern("crews a vehicle")]
    CrewsAVehicle,

    [RegexPattern("is tapped for mana")]
    IsTappedForMana,

    IsDealtCombatDamage,

    IsDealtDamage,

    [RegexPattern("deals combat damage to a player or planeswalker", "deal combat damage to a player or planeswalker")]
    DealsCombatDamageToAPlayerOrPlaneswalker,

    [RegexPattern("deals combat damage to a player or battle", "deal combat damage to a player or battle")]
    DealsCombatDamageToAPlayerOrBattle,

    [RegexPattern("deals combat damage to a player", "deal combat damage to a player")]
    DealsCombatDamageToAPlayer,

    [RegexPattern("deals combat damage to an opponent", "deal combat damage to an opponent")]
    DealsCombatDamageToAnOpponent,

    [RegexPattern("deals combat damage to you")]
    DealsCombatDamageToYou,

    [RegexPattern("deals combat damage to enchanted player")]
    DealsCombatDamageToEnchantedPlayer,

    [RegexPattern("deals combat damage to a creature")]
    DealsCombatDamageToACreature,

    [RegexPattern("deals combat damage", "deal combat damage")]
    DealsCombatDamage,

    [RegexPattern("deals damage to a creature or opponent")]
    DealsDamageToACreatureOrOpponent,

    [RegexPattern("deals damage to a player", "deal damage to a player")]
    DealsDamageToAPlayer,

    [RegexPattern("deals damage to an opponent", "deal damage to an opponent")]
    DealsDamageToAnOpponent,

    [RegexPattern("deals damage to you")]
    DealsDamageToYou,

    [RegexPattern("deals damage to a creature")]
    DealsDamageToACreature,

    [RegexPattern("deals damage to one or more creatures")]
    DealsDamageToOneOrMoreCreatures,

    [RegexPattern("deals damage", "deal damage")]
    DealsDamage
}
