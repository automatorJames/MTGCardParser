namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Remove Enchantments, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Remove Enchantments</exampledoc>
/// <examplecapture>then destroy all other enchantments you control, all other auras attached to permanents you control, and all other auras attached to attacking creatures your opponents control</examplecapture>
[TokenizationOrder(-1)]
public class RemoveEnchantmentsRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then destroy all other enchantments you control, all other auras attached to permanents you control, and all other auras attached to attacking creatures your opponents control"];
}
