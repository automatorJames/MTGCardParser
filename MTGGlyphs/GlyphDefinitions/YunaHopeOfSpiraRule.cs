namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Yuna, Hope of Spira, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Yuna, Hope of Spira</exampledoc>
[TokenizationOrder(-1)]
public class YunaHopeOfSpiraRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during your turn, yuna and enchantment creatures you control have trample, lifelink, and ward {2}"];
}
