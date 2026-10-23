namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Scrying Glass, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scrying Glass</exampledoc>
[TokenizationOrder(-1)]
public class ScryingGlassRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that opponent reveals exactly the chosen number of cards of the chosen color, you draw a card"];
}
