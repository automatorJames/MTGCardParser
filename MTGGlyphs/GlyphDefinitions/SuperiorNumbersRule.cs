namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Superior Numbers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Superior Numbers</exampledoc>
[TokenizationOrder(-1)]
public class SuperiorNumbersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals damage to target creature equal to the number of creatures you control in excess of the number of creatures target opponent controls"];
}
