namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Falling Star, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Falling Star</exampledoc>
[TokenizationOrder(-1)]
public class FallingStarRule4 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if", Nib.This, "doesn't turn completely over at least once during the flip, it has no effect"];
}
