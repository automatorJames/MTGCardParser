namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Necromancy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Necromancy</exampledoc>
[TokenizationOrder(-1)]
public class NecromancyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this enchantment enters, if it's on the battlefield, it becomes an aura with \"enchant creature put onto the battlefield with", Nib.This];
}
