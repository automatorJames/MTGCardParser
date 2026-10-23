namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vincent's Limit Break, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vincent's Limit Break</exampledoc>
/// <examplecapture>until end of turn, target creature you control gains "when this creature dies, return it to the battlefield tapped under its owner's control" and has the chosen base power and toughness</examplecapture>
[TokenizationOrder(-1)]
public class VincentsLimitBreakRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, target creature you control gains \"when this creature dies, return it to the battlefield tapped under its owner's control\" and has the chosen base power and toughness"];
}
