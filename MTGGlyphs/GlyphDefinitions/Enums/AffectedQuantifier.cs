namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How many of the objects an effect just acted on a later effect takes, before "… this way": "a creature card put into your graveyard this way", "each card put into a graveyard this way", "all …".</summary>
public enum AffectedQuantifier
{
    [RegexPattern("an?")]
    A,

    Each,

    All
}
