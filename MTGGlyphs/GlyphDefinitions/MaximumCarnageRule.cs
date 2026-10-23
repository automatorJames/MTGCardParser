namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Maximum Carnage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Maximum Carnage</exampledoc>
[TokenizationOrder(-1)]
public class MaximumCarnageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["i — until your next turn, each creature attacks each combat if able and attacks a player other than you if able"];
}
