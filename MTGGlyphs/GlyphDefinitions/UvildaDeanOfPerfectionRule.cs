namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Uvilda, Dean of Perfection // Nassari, Dean of Expression, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Uvilda, Dean of Perfection // Nassari, Dean of Expression</exampledoc>
/// <examplecapture>it gains "at the beginning of your upkeep, if this card is exiled, remove a refine counter from it" and "when the last refine counter is removed from this card, if it's exiled, you may cast it</examplecapture>
[TokenizationOrder(-1)]
public class UvildaDeanOfPerfectionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gains \"at the beginning of your upkeep, if this card is exiled, remove a refine counter from it\" and \"when the last refine counter is removed from this card, if it's exiled, you may cast it"];
}
