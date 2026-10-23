namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Crackle with Power, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Crackle with Power</exampledoc>
[TokenizationOrder(-1)]
public class CrackleWithPowerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals five times x damage to each of up to x targets"];
}
