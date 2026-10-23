namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nissa, Genesis Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nissa, Genesis Mage</exampledoc>
[Dependent]
public class NissaGenesisMageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["untap up to two target creatures and up to two target lands"];
}
