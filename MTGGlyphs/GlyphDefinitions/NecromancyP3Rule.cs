namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class NecromancyP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this enchantment enters, if it's on the battlefield, it becomes an aura with \"enchant creature put onto the battlefield with", Nib.This];
}
