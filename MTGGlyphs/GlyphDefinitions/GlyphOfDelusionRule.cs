namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Glyph of Delusion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glyph of Delusion</exampledoc>
[TokenizationOrder(-1)]
public class GlyphOfDelusionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put x glyph counters on target creature that target wall blocked this turn, where x is the power of that blocked creature"];
}
