namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fasting, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fasting</exampledoc>
[TokenizationOrder(-1)]
public class FastingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would begin your draw step, you may skip that step instead"];
}
