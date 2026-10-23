namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Alchemist's Retrieval, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alchemist's Retrieval</exampledoc>
[TokenizationOrder(-1)]
public class AlchemistsRetrievalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target nonland permanent [you control] to its owner's hand"];
}
