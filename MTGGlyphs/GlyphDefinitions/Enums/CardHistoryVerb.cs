namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a player has done to the spells or cards being counted this turn, as a past participle: "spells you've cast this turn", "cards you've discarded this turn", "cards that player has drawn this turn", "cards you've cycled or discarded this turn", "sacrificed".</summary>
public enum CardHistoryVerb
{
    [RegexPattern("cast")]
    Cast,

    [RegexPattern("drawn")]
    Drawn,

    [RegexPattern("cycled or discarded")]
    CycledOrDiscarded,

    [RegexPattern("discarded")]
    Discarded,

    [RegexPattern("sacrificed")]
    Sacrificed
}
