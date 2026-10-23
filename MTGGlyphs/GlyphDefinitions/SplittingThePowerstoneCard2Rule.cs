namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Splitting the Powerstone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Splitting the Powerstone</exampledoc>
[TokenizationOrder(-1)]
public class SplittingThePowerstoneCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the sacrificed artifact was legendary, draw a card"];
}
