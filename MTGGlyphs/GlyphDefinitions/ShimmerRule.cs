namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shimmer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shimmer</exampledoc>
[TokenizationOrder(-1)]
public class ShimmerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each land of the chosen type has phasing"];
}
