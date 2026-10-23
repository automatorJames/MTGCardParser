namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Worms of the Earth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Worms of the Earth</exampledoc>
[TokenizationOrder(-1)]
public class WormsOfTheEarthCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a player does either, destroy this enchantment"];
}
