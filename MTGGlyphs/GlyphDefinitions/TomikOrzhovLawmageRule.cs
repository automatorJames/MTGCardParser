namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tomik, Orzhov Lawmage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tomik, Orzhov Lawmage</exampledoc>
[TokenizationOrder(-1)]
public class TomikOrzhovLawmageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["planeswalkers you control have \"no more than one creature can attack this planeswalker each combat"];
}
