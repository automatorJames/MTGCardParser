namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tropical Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tropical Storm</exampledoc>
[TokenizationOrder(-1)]
public class TropicalStormRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals x damage to each creature with flying and 1 additional damage to each blue creature"];
}
