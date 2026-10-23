namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Diabolic Servitude, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Diabolic Servitude</exampledoc>
[Dependent]
public class DiabolicServitudeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile it and return this enchantment to its owner's hand"];
}
