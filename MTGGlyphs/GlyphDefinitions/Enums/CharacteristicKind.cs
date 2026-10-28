namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A kind of characteristic as a plural, as what a permanent loses when it is re-typed: "loses all other card types and creature types", "loses all other colors, card types, and creature types", "loses all other abilities".</summary>
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
