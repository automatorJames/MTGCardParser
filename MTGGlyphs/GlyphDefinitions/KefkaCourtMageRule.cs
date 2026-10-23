namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kefka, Court Mage // Kefka, Ruler of Ruin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kefka, Court Mage // Kefka, Ruler of Ruin</exampledoc>
[TokenizationOrder(-1)]
public class KefkaCourtMageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then you draw a card for each card type among cards discarded this way"];
}
