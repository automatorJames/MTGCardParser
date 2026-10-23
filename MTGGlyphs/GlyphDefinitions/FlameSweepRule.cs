namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Flame Sweep, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flame Sweep</exampledoc>
[TokenizationOrder(-1)]
public class FlameSweepRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 2 damage to each creature except for creatures you control with flying"];
}
