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
    Attacks
}
