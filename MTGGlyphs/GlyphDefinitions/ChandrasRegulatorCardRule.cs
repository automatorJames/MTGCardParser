namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chandra's Regulator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra's Regulator</exampledoc>
[TokenizationOrder(-1)]
public class ChandrasRegulatorCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{1}, {t}, discard a mountain card or a red card: draw a card"];
}
