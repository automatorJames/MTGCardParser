namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thieves' Auction, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thieves' Auction</exampledoc>
[TokenizationOrder(-1)]
public class ThievesAuctionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["starting with you, each player chooses one of the exiled cards and puts it onto the battlefield tapped under their control"];
}
