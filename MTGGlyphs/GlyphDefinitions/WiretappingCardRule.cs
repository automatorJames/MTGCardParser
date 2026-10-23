namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wiretapping, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wiretapping</exampledoc>
[TokenizationOrder(-1)]
public class WiretappingCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you draw your first card during each of your draw steps, draw a card"];
}
