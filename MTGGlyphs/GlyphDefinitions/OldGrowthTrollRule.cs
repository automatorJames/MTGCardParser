namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Old-Growth Troll, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Old-Growth Troll</exampledoc>
/// <examplecapture>it's an aura enchantment with enchant forest you control and "enchanted forest has '{t}: add {g}{g}' and '{1}, {t}, sacrifice this land: create a tapped 4/4 green troll warrior creature token with trample</examplecapture>
[TokenizationOrder(-1)]
public class OldGrowthTrollRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it's an aura enchantment with enchant forest you control and \"enchanted forest has '{t}: add {g}{g}' and '{1}, {t}, sacrifice this land: create a tapped 4/4 green troll warrior creature token with trample"];
}
