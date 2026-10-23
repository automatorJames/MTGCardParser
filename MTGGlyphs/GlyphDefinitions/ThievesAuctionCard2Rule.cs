namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thieves' Auction, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thieves' Auction</exampledoc>
[TokenizationOrder(-1)]
public class ThievesAuctionCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["repeat this process until all cards exiled this way have been chosen"];
}
