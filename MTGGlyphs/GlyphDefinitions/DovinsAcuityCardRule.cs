namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dovin's Acuity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dovin's Acuity</exampledoc>
[TokenizationOrder(-1)]
public class DovinsAcuityCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you cast an instant spell during your main phase, you may return this enchantment to its owner's hand"];
}
