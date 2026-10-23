namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sidequest: Catch a Fish // Cooking Campsite, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sidequest: Catch a Fish // Cooking Campsite</exampledoc>
[TokenizationOrder(-1)]
public class SidequestCatchAFishRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you put a card into your hand this way, create a food token and transform this enchantment"];
}
