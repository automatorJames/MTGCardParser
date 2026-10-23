namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hollow Marauder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hollow Marauder</exampledoc>
[TokenizationOrder(-1)]
public class HollowMarauderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each of those opponents who didn't discard a card with mana value 4 or greater, draw a card"];
}
