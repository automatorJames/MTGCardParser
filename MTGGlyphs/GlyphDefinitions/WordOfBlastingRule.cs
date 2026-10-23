namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Word of Blasting, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Word of Blasting</exampledoc>
[TokenizationOrder(-1)]
public class WordOfBlastingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals damage equal to that wall's mana value to the wall's controller"];
}
