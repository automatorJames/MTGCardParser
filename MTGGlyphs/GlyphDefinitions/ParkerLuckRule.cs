namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Parker Luck, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Parker Luck</exampledoc>
[TokenizationOrder(-1)]
public class ParkerLuckRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they each lose life equal to the mana value of the card revealed by the other player"];
}
