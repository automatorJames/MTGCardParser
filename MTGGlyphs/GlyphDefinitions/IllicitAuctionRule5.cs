namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Illicit Auction, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illicit Auction</exampledoc>
[TokenizationOrder(-1)]
public class IllicitAuctionRule5 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the high bidder loses life equal to the high bid and gains control of the creature"];
}
