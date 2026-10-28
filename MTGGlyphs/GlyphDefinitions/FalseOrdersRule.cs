namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of False Orders, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>False Orders</exampledoc>
[TokenizationOrder(-1)]
public class FalseOrdersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures it was blocking that had become blocked by only that creature this combat become unblocked"];
}
