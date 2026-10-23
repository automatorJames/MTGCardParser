namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Malignant Growth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Malignant Growth</exampledoc>
/// <examplecapture>that player draws an additional card for each growth counter on this enchantment, then this enchantment deals damage to the player equal to the number of cards they drew this way</examplecapture>
[Dependent]
public class MalignantGrowthEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player draws an additional card for each growth counter on this enchantment, then this enchantment deals damage to the player equal to the number of cards they drew this way"];
}
