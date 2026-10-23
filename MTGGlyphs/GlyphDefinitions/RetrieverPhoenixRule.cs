namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Retriever Phoenix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Retriever Phoenix</exampledoc>
[TokenizationOrder(-1)]
public class RetrieverPhoenixRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as this card is in your graveyard, if you would learn, you may instead return this card to the battlefield"];
}
