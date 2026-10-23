namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tom, Bert, and William, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tom, Bert, and William</exampledoc>
[TokenizationOrder(-1)]
public class TomBertAndWilliam3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they're an artifact"];
}
