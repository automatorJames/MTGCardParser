namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Illuminated Folio, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illuminated Folio</exampledoc>
[TokenizationOrder(-1)]
public class IlluminatedFolioCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{1}, {t}, reveal two cards from your hand that share a color: draw a card"];
}
