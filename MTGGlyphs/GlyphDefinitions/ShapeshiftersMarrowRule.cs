namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shapeshifter's Marrow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shapeshifter's Marrow</exampledoc>
[TokenizationOrder(-1)]
public class ShapeshiftersMarrowRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's a creature card, the player puts the card into their graveyard and this enchantment becomes a copy of that card"];
}
