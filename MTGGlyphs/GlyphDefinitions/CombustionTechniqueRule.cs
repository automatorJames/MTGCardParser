namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Combustion Technique, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Combustion Technique</exampledoc>
[TokenizationOrder(-1)]
public class CombustionTechniqueRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals damage equal to 2 plus the number of lesson cards in your graveyard to target creature"];
}
