namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Heartwarming Redemption, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Heartwarming Redemption</exampledoc>
[TokenizationOrder(-1)]
public class HeartwarmingRedemptionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["discard all the cards in your hand, then draw that many cards plus one"];
}
