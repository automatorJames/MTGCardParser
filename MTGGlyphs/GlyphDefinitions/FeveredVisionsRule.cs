namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fevered Visions, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fevered Visions</exampledoc>
[TokenizationOrder(-1)]
public class FeveredVisionsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the player is your opponent and has four or more cards in hand, this enchantment deals 2 damage to that player"];
}
