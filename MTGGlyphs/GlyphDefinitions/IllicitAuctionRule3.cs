namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Illicit Auction, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illicit Auction</exampledoc>
/// <examplecapture>in turn order, each player may top the high bid</examplecapture>
[TokenizationOrder(-1)]
public class IllicitAuctionRule3 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["in turn order, each player may top the high bid"];
}
