namespace MTGGlyphs.GlyphDefinitions;

public enum PlayerEventKind
{
    [RegexPattern("gains? life")]
    GainsLife,

    [RegexPattern("loses? life")]
    LosesLife,

    [RegexPattern("gains? or loses? life")]
    GainsOrLosesLife,

    [RegexPattern("pays? life")]
    PaysLife,

    [RegexPattern("draws? (your|their) second card each turn")]
    DrawsSecondCardEachTurn,

    [RegexPattern("cycles? or discards? (a|another) card")]
    CyclesOrDiscardsACard,

    [RegexPattern("discards? (a card|one or more cards)")]
    DiscardsACard,

    [RegexPattern("cycles? a card")]
    CyclesACard,

    [RegexPattern("cycles? another card")]
    CyclesAnotherCard,

    [RegexPattern("cycles? this card")]
    CyclesThisCard,

    [RegexPattern("sacrifices? another permanent")]
    SacrificesAnotherPermanent,

    [RegexPattern("scry or surveil", "scries or surveils")]
    ScriesOrSurveils,

    [RegexPattern("surveils?")]
    Surveils,

    [RegexPattern("scry|scries")]
    Scries,

    [RegexPattern("commits? a crime")]
    CommitsACrime,

    [RegexPattern("attacks?")]
    Attacks,

    [RegexPattern("wins? a coin flip")]
    WinsACoinFlip,

    [RegexPattern("loses? a coin flip")]
    LosesACoinFlip,

    [RegexPattern("clash(?:es)? and wins?")]
    ClashesAndWins,

    [RegexPattern("rolls? one or more dice")]
    RollsOneOrMoreDice,

    [RegexPattern(@"gets? one or more \{e\} \(energy counters\)")]
    GetsEnergy,

    [RegexPattern("activates? an exhaust ability")]
    ActivatesExhaustAbility,

    [RegexPattern("activates? a boast ability")]
    ActivatesBoastAbility,

    [RegexPattern("plays? a land or casts? a spell")]
    PlaysALandOrCastsASpell,

    [RegexPattern("plays? a land")]
    PlaysALand,

    [RegexPattern("shuffles? (your|their) library")]
    ShufflesLibrary,

    [RegexPattern("search(?:es)? (your|their) library")]
    SearchesLibrary,

    [RegexPattern("kicks? a spell")]
    KicksASpell,

    [RegexPattern("investigates?")]
    Investigates,

    [RegexPattern("completes? a dungeon")]
    CompletesADungeon,

    [RegexPattern("gives? a gift")]
    GivesAGift,

    [RegexPattern("turns? a permanent face up")]
    TurnsAPermanentFaceUp,

    [RegexPattern("manifests? dread")]
    ManifestsDread,

    [RegexPattern("(?:is|are) dealt noncombat damage")]
    IsDealtNoncombatDamage,

    [RegexPattern("(?:is|are) dealt combat damage")]
    IsDealtCombatDamage,

    [RegexPattern("(?:is|are) dealt damage")]
    IsDealtDamage
}
