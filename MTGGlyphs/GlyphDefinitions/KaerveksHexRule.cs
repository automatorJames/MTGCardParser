namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kaervek's Hex, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaervek's Hex</exampledoc>
[TokenizationOrder(-1)]
public class KaerveksHexRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 1 damage to each nonblack creature and an additional 1 damage to each green creature"];
}
