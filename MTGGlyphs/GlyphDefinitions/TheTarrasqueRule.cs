namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Tarrasque, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Tarrasque</exampledoc>
[TokenizationOrder(-1)]
public class TheTarrasqueRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "has haste and ward {10} as long as it was cast"];
}
