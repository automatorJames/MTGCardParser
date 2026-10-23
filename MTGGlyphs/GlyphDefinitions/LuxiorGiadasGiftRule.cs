namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Luxior, Giada's Gift, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Luxior, Giada's Gift</exampledoc>
[TokenizationOrder(-1)]
public class LuxiorGiadasGiftRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped permanent isn't a planeswalker and is a creature in addition to its other types"];
}
