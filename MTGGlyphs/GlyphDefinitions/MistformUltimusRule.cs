namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mistform Ultimus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mistform Ultimus</exampledoc>
[TokenizationOrder(-1)]
public class MistformUltimusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "is every creature type (even if this card isn't on the battlefield)"];
}
