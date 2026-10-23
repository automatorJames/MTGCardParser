namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Parker Luck, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Parker Luck</exampledoc>
[TokenizationOrder(-1)]
public class ParkerLuckCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then they each put the card they revealed into their hand"];
}
