namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sorrow's Path, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sorrow's Path</exampledoc>
[TokenizationOrder(-1)]
public class SorrowsPathCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each one then blocks all creatures the other was blocking"];
}
