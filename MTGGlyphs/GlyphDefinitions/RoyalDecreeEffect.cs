namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Royal Decree, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Royal Decree</exampledoc>
[Dependent]
public class RoyalDecreeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["mountain, black permanent, or red permanent becomes tapped, this enchantment deals 1 damage to that permanent's controller"];
}
