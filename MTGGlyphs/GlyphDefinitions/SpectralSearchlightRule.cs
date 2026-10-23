namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spectral Searchlight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spectral Searchlight</exampledoc>
[TokenizationOrder(-1)]
public class SpectralSearchlightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player adds one mana of any color they choose"];
}
