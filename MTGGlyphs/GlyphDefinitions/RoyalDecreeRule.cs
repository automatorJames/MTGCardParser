namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Royal Decree, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Royal Decree</exampledoc>
[TokenizationOrder(-1)]
public class RoyalDecreeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a swamp, mountain, black permanent, or red permanent becomes tapped, this enchantment deals 1 damage to that permanent's controller"];
}
