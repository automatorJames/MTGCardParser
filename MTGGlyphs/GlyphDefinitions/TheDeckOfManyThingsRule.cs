namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Deck of Many Things, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Deck of Many Things</exampledoc>
[TokenizationOrder(-1)]
public class TheDeckOfManyThingsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the result is 0 or less, discard your hand"];
}
