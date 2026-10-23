namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Read the Runes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Read the Runes</exampledoc>
[TokenizationOrder(-1)]
public class ReadTheRunesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each card drawn this way, discard a card unless you sacrifice a permanent"];
}
