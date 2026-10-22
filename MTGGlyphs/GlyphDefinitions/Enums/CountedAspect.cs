namespace MTGGlyphs.GlyphDefinitions;

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
