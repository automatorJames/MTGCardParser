namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Illicit Auction, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illicit Auction</exampledoc>
[TokenizationOrder(-1)]
public class IllicitAuctionCard5Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you start the bidding with a bid of 0"];
}
