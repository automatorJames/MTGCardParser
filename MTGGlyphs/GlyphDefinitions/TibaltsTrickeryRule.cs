namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tibalt's Trickery, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tibalt's Trickery</exampledoc>
[TokenizationOrder(-1)]
public class TibaltsTrickeryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose 1, 2, or 3 at random"];
}
