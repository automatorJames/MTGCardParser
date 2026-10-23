namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cosmos Charger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cosmos Charger</exampledoc>
[TokenizationOrder(-1)]
public class CosmosChargerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["foretelling cards from your hand costs {1} less and can be done on any player's turn"];
}
