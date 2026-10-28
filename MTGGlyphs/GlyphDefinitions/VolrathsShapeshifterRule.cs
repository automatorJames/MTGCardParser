namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Volrath's Shapeshifter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Volrath's Shapeshifter</exampledoc>
[TokenizationOrder(-1)]
public class VolrathsShapeshifterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as the top card of your graveyard is a creature card, this creature has the full text of that card and has the text \"{2}: discard a card"];
}
