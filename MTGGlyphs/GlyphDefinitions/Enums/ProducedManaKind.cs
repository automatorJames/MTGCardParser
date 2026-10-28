namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Mana described by kind rather than by symbol, as what a land produces instead: "colorless mana", "mana of a color of your choice" ("it produces colorless mana instead of any other type and amount").</summary>
public enum ProducedManaKind
{
    [RegexPattern("colorless mana")]
    Colorless,

    [RegexPattern("(one )?mana of a color of your choice")]
    ChosenColor
}
