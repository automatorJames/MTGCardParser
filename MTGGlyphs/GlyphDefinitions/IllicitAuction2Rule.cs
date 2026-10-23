namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Illicit Auction, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illicit Auction</exampledoc>
[TokenizationOrder(-1)]
public class IllicitAuction2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the bidding ends if the high bid stands"];
}
