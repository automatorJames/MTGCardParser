namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Falling Star, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Falling Star</exampledoc>
[TokenizationOrder(-1)]
public class FallingStarCard4Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap all creatures dealt damage by", Nib.This];
}
