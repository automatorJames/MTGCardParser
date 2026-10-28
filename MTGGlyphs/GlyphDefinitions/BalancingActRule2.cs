namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Balancing Act, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Balancing Act</exampledoc>
[TokenizationOrder(-1)]
public class BalancingActRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player discards cards the same way"];
}
