namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Farideh, Devil's Chosen, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Farideh, Devil's Chosen</exampledoc>
[TokenizationOrder(-1)]
public class FaridehDevilsChosenRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if any of those results was 10 or higher, draw a card"];
}
