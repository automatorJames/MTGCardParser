namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kang the Conqueror, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kang the Conqueror</exampledoc>
[TokenizationOrder(-1)]
public class KangTheConquerorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during that turn, power-up abilities can't be activated"];
}
