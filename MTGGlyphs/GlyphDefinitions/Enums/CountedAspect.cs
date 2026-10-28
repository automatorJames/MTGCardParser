namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A characteristic whose distinct values are counted, as domain, delirium and converge count them: "basic land types", "colors", "card types", "permanent types", "mana values" - "there are four or more card types among cards in your graveyard", "for each color".</summary>
public enum CountedAspect
{
    [RegexPattern("basic land types?")]
    BasicLandTypes,

    [RegexPattern("colors?")]
    Colors,

    [RegexPattern("card types?")]
    CardTypes,

    [RegexPattern("permanent types?")]
    PermanentTypes,

    [RegexPattern("(?:different )?mana values?")]
    ManaValues
}
