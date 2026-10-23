namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Auratouched Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Auratouched Mage</exampledoc>
[TokenizationOrder(-1)]
public class AuratouchedMageCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise, reveal the aura card and put it into your hand"];
}
