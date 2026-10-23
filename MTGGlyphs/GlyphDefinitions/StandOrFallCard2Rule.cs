namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Stand or Fall, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stand or Fall</exampledoc>
[TokenizationOrder(-1)]
public class StandOrFallCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only creatures in the chosen piles can block this turn"];
}
