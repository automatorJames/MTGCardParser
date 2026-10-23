namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Venser's Diffusion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Venser's Diffusion</exampledoc>
[TokenizationOrder(-1)]
public class VensersDiffusionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target nonland permanent or suspended card to its owner's hand"];
}
