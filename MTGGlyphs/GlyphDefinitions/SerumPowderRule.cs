namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Serum Powder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Serum Powder</exampledoc>
/// <examplecapture>any time you could mulligan and this card is in your hand, you may exile all the cards from your hand, then draw that many cards</examplecapture>
[TokenizationOrder(-1)]
public class SerumPowderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any time you could mulligan and this card is in your hand, you may exile all the cards from your hand, then draw that many cards"];
}
