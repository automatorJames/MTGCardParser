namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Trial of Agony, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Trial of Agony</exampledoc>
[TokenizationOrder(-1)]
public class TrialOfAgonyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 5 damage to that creature, and the other can't block this turn"];
}
