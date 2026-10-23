namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sorrow's Path, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sorrow's Path</exampledoc>
[TokenizationOrder(-1)]
public class SorrowsPathRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if each of those creatures could block all creatures that the other is blocking, remove both of them from combat"];
}
