namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kaervek's Torch, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaervek's Torch</exampledoc>
[TokenizationOrder(-1)]
public class KaerveksTorchRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as", Nib.This, "is on the stack, spells that target it cost {2} more to cast"];
}
