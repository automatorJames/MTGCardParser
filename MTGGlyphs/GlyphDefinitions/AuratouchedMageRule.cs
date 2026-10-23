namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Auratouched Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Auratouched Mage</exampledoc>
[TokenizationOrder(-1)]
public class AuratouchedMageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this creature is still on the battlefield, put that aura card onto the battlefield attached to it"];
}
