namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dovin's Acuity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dovin's Acuity</exampledoc>
[Dependent]
public class DovinsAcuityEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may return this enchantment to its owner's hand"];
}
