namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The mana noun of a mana-spent condition: plain "mana" ("at least four mana was spent to cast it"), "mana of the same color" ("at least three mana of the same color was spent to cast it"), or "colors of mana" ("two or more colors of mana were spent to cast it").</summary>
public enum SpentManaPhrase
{
    [RegexPattern("mana of the same color")]
    ManaOfOneColor,

    [RegexPattern("colors of mana")]
    ColorsOfMana,

    Mana
}
