namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Isperia the Inscrutable, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Isperia the Inscrutable</exampledoc>
/// <examplecapture>if a card with the chosen name is revealed this way, search your library for a creature card with flying, reveal it, put it into your hand, then shuffle</examplecapture>
[TokenizationOrder(-1)]
public class IsperiaTheInscrutableRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a card with the chosen name is revealed this way, search your library for a creature card with flying, reveal it, put it into your hand, then shuffle"];
}
