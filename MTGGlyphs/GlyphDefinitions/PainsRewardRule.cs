namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pain's Reward, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pain's Reward</exampledoc>
[TokenizationOrder(-1)]
public class PainsRewardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player may bid life"];
}
