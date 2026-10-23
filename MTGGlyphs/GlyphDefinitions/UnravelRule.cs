namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Unravel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unravel</exampledoc>
[TokenizationOrder(-1)]
public class UnravelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the amount of mana spent to cast that spell was less than its mana value, you draw a card"];
}
