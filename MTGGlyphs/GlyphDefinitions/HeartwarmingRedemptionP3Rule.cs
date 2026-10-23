namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class HeartwarmingRedemptionP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["discard all the cards in your hand, then draw that many cards plus one"];
}
