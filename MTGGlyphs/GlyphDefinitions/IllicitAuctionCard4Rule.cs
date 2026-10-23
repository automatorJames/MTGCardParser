namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Illicit Auction, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illicit Auction</exampledoc>
[TokenizationOrder(-1)]
public class IllicitAuctionCard4Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player may bid life for control of target creature"];
}
