namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Deck of Many Things, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Deck of Many Things</exampledoc>
[Dependent]
public class TheDeckOfManyThingsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["roll a d20 and subtract the number of cards in your hand"];
}
