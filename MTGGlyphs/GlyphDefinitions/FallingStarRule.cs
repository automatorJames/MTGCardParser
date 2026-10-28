namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Falling Star, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Falling Star</exampledoc>
[TokenizationOrder(-1)]
public class FallingStarRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flip", Nib.This, "onto the playing area from a height of at least one foot"];
}
