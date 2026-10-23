namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Winged Portent, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Winged Portent</exampledoc>
[TokenizationOrder(-1)]
public class WingedPortentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card for each creature you control [with flying]"];
}
