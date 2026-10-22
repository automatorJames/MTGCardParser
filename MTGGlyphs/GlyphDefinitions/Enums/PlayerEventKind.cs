namespace MTGGlyphs.GlyphDefinitions;

public enum PlayerEventKind
{
    [RegexPattern("gains? life")]
    GainsLife,

    [RegexPattern("loses? life")]
    LosesLife,

    [RegexPattern("draws? (your|their) second card each turn")]
    DrawsSecondCardEachTurn,

    [RegexPattern("cycles? or discards? (a|another) card")]
    CyclesOrDiscardsACard,

    [RegexPattern("discards? (a card|one or more cards)")]
    DiscardsACard,

    [RegexPattern("cycles? a card")]
    CyclesACard,

    [RegexPattern("cycles? this card")]
    CyclesThisCard,

    [RegexPattern("sacrifices? another permanent")]
    SacrificesAnotherPermanent,

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

    [RegexPattern("rolls? one or more dice")]
    RollsOneOrMoreDice,

    [RegexPattern(@"gets? one or more \{e\} \(energy counters\)")]
    GetsEnergy,

    [RegexPattern("activates? an exhaust ability")]
    ActivatesExhaustAbility,

    [RegexPattern("plays? a land")]
    PlaysALand,

    [RegexPattern("shuffles? (your|their) library")]
    ShufflesLibrary,

    [RegexPattern("investigates?")]
    Investigates,

    [RegexPattern("(?:is|are) dealt noncombat damage")]
    IsDealtNoncombatDamage,

    [RegexPattern("(?:is|are) dealt combat damage")]
    IsDealtCombatDamage,

    [RegexPattern("(?:is|are) dealt damage")]
    IsDealtDamage
}
