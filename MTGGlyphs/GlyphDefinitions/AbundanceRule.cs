namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Abundance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Abundance</exampledoc>
/// <examplecapture>if you would draw a card, you may instead choose land or nonland and reveal cards from the top of your library until you reveal a card of the chosen kind</examplecapture>
[TokenizationOrder(-1)]
public class AbundanceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would draw a card, you may instead choose land or nonland and reveal cards from the top of your library until you reveal a card of the chosen kind"];
}
