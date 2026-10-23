namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Monstrous Emergence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Monstrous Emergence</exampledoc>
[TokenizationOrder(-1)]
public class MonstrousEmergenceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals damage equal to the power of the creature you chose or the card you revealed to target creature"];
}
