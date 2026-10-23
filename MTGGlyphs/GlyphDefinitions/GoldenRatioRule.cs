namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Golden Ratio, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Golden Ratio</exampledoc>
[TokenizationOrder(-1)]
public class GoldenRatioRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card for each different power among creatures you control"];
}
