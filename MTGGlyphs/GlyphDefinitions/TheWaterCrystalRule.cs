namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Water Crystal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Water Crystal</exampledoc>
[TokenizationOrder(-1)]
public class TheWaterCrystalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an opponent would mill one or more cards, they mill that many cards plus four instead"];
}
