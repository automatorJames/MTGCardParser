namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hex Magic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hex Magic</exampledoc>
[TokenizationOrder(-1)]
public class HexMagicRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all the cards from your hand, then draw that many cards"];
}
