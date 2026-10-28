namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tibalt's Trickery, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tibalt's Trickery</exampledoc>
/// <examplecapture>its controller mills that many cards, then exiles cards from the top of their library until they exile a nonland card with a different name than that spell</examplecapture>
[TokenizationOrder(-1)]
public class TibaltsTrickeryRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its controller mills that many cards, then exiles cards from the top of their library until they exile a nonland card with a different name than that spell"];
}
