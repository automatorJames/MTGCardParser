namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How much mana "add" adds when it isn't a fixed symbol: "an additional" (a mana trigger's extra mana, "its controller adds an additional {g}") or "that much" (as much as an amount named earlier, "add that much {g}").</summary>
public enum AddedManaAmount
{
    [RegexPattern("an additional")]
    Additional,

    [RegexPattern("that much")]
    ThatMuch
}
