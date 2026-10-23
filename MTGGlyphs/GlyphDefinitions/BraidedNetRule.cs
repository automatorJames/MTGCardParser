namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Braided Net // Braided Quipu, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Braided Net // Braided Quipu</exampledoc>
[TokenizationOrder(-1)]
public class BraidedNetRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its activated abilities can't be activated for as long as it remains tapped"];
}
