namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Haktos the Unscarred, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Haktos the Unscarred</exampledoc>
[TokenizationOrder(-1)]
public class HaktosTheUnscarredRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as haktos enters, choose 2, 3, or 4 at random"];
}
