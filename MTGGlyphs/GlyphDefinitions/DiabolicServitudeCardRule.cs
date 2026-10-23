namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Diabolic Servitude, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Diabolic Servitude</exampledoc>
[TokenizationOrder(-1)]
public class DiabolicServitudeCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when the creature put onto the battlefield with this enchantment dies, exile it and return this enchantment to its owner's hand"];
}
