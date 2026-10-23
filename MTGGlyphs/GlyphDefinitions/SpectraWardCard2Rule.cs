namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spectra Ward, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spectra Ward</exampledoc>
[TokenizationOrder(-1)]
public class SpectraWardCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this effect doesn't remove auras"];
}
