namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pain's Reward, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pain's Reward</exampledoc>
[TokenizationOrder(-1)]
public class PainsRewardCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you start the bidding with a bid of any number"];
}
