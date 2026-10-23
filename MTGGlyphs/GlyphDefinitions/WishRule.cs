namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wish, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wish</exampledoc>
[TokenizationOrder(-1)]
public class WishRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may play a card you own from outside the game this turn"];
}
