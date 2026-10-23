namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pain's Reward, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pain's Reward</exampledoc>
[TokenizationOrder(-1)]
public class PainsRewardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the high bidder loses life equal to the high bid and draws four cards"];
}
