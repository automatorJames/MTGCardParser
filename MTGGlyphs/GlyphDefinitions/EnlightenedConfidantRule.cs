namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Enlightened Confidant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Enlightened Confidant</exampledoc>
/// <examplecapture>if you put a card with mana value less than or equal to the amount of life you gained this turn into your graveyard this way, put that card into your hand</examplecapture>
[TokenizationOrder(-1)]
public class EnlightenedConfidantRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you put a card with mana value less than or equal to the amount of life you gained this turn into your graveyard this way, put that card into your hand"];
}
