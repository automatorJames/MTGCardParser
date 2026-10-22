namespace MTGGlyphs.GlyphDefinitions;

public enum SpentManaPhrase
{
    [RegexPattern("mana of the same color")]
    ManaOfOneColor,

    [RegexPattern("colors of mana")]
    ColorsOfMana,

    Mana
}
