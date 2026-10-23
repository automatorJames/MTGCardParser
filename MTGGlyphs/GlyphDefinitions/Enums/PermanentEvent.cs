namespace MTGGlyphs.GlyphDefinitions;

public enum PermanentEvent
{
    [RegexPattern("dies", "die")]
    Dies,

    [RegexPattern("enters untapped")]
    EntersUntapped,

    [RegexPattern("enters from your graveyard")]
    EntersFromYourGraveyard,

    [RegexPattern("enters from a graveyard")]
    EntersFromAGraveyard,

    [RegexPattern("enters", "enter")]
    EntersTheBattlefield,

    [RegexPattern("leaves the battlefield without dying", "leave the battlefield without dying")]
    LeavesTheBattlefieldWithoutDying,

    [RegexPattern("leaves the battlefield", "leave the battlefield")]
    LeavesTheBattlefield,

    IsPutIntoAGraveyardFromTheBattlefield,

    IsPutIntoAGraveyardFromAnywhere,

    IsPutIntoAGraveyard,

    IsPutIntoExileFromTheBattlefield,

    [RegexPattern("is returned to your hand from the battlefield")]
    IsReturnedToYourHandFromTheBattlefield,

    [RegexPattern("is returned to your hand")]
    IsReturnedToYourHand,

    [RegexPattern("is returned to a player's hand")]
    IsReturnedToAPlayersHand,

    [RegexPattern("is returned to its owner's hand", "are returned to hand")]
    IsReturnedToHand,

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

    [RegexPattern("attacks a battle", "attack a battle")]
    AttacksABattle,

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

    [RegexPattern("trains")]
    Trains,

    [RegexPattern("exploits a creature")]
    ExploitsACreature,

    [RegexPattern("enlists a creature")]
    EnlistsACreature,

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

    [RegexPattern("deals combat damage to one of your opponents", "deal combat damage to one of your opponents")]
    DealsCombatDamageToOneOfYourOpponents,

    [RegexPattern("deals combat damage to you")]
    DealsCombatDamageToYou,

    [RegexPattern("deals combat damage to enchanted player")]
    DealsCombatDamageToEnchantedPlayer,

    [RegexPattern("deals combat damage to a creature")]
    DealsCombatDamageToACreature,

    [RegexPattern("deals combat damage to one or more blocking creatures")]
    DealsCombatDamageToOneOrMoreBlockingCreatures,

    [RegexPattern("deals combat damage", "deal combat damage")]
    DealsCombatDamage,

    [RegexPattern("deals damage to a creature or opponent")]
    DealsDamageToACreatureOrOpponent,

    [RegexPattern("deals damage to a player or planeswalker", "deal damage to a player or planeswalker")]
    DealsDamageToAPlayerOrPlaneswalker,

    [RegexPattern("deals damage to a player", "deal damage to a player")]
    DealsDamageToAPlayer,

    [RegexPattern("deals damage to an opponent or battle", "deal damage to an opponent or battle")]
    DealsDamageToAnOpponentOrBattle,

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
