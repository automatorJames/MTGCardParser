namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Caged Sun, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Caged Sun</exampledoc>
[TokenizationOrder(-1)]
public class CagedSunRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a land's ability causes you to add one or more mana of the chosen color, add an additional one mana of that color"];
}
