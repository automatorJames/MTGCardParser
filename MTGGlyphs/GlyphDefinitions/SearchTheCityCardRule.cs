namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Search the City, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Search the City</exampledoc>
[TokenizationOrder(-1)]
public class SearchTheCityCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you play a card with the same name as one of the exiled cards, you may put one of those cards with that name into its owner's hand"];
}
