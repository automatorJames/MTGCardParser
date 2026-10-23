namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Flame Discharge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flame Discharge</exampledoc>
[TokenizationOrder(-1)]
public class FlameDischargeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you controlled a modified creature as you cast this spell, it deals x plus 2 damage instead"];
}
