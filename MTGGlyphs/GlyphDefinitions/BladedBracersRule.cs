namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bladed Bracers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bladed Bracers</exampledoc>
[TokenizationOrder(-1)]
public class BladedBracersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as equipped creature is a human or an angel, it has vigilance"];
}
