namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Diabolic Servitude, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Diabolic Servitude</exampledoc>
[Dependent]
public class DiabolicServitude2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile the creature put onto the battlefield with this enchantment"];
}
