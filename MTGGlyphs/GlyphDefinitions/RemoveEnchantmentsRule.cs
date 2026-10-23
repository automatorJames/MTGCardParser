namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Remove Enchantments, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Remove Enchantments</exampledoc>
/// <examplecapture>return to your hand all enchantments you both own and control, all auras you own attached to permanents you control, and all auras you own attached to attacking creatures your opponents control</examplecapture>
[TokenizationOrder(-1)]
public class RemoveEnchantmentsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return to your hand all enchantments you both own and control, all auras you own attached to permanents you control, and all auras you own attached to attacking creatures your opponents control"];
}
