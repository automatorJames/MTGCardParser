namespace MTGGlyphs.GlyphDefinitions;

public enum CharacteristicKind
{
    Colors,

    [RegexPattern("card types")]
    CardTypes,

    [RegexPattern("creature types")]
    CreatureTypes,

    [RegexPattern("land types")]
    LandTypes,

    Names,

    Abilities
}
