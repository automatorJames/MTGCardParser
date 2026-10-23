namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Glimpse the Cosmos, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glimpse the Cosmos</exampledoc>
[TokenizationOrder(-1)]
public class GlimpseTheCosmosRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you cast this card this way and it would be put into your graveyard, exile it instead"];
}
