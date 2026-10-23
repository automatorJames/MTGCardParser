namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Delina, Wild Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Delina, Wild Mage</exampledoc>
[TokenizationOrder(-1)]
public class DelinaWildMageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may roll again"];
}
