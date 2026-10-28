namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sylvan Library, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sylvan Library</exampledoc>
[TokenizationOrder(-1)]
public class SylvanLibraryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, choose two cards in your hand drawn this turn"];
}
