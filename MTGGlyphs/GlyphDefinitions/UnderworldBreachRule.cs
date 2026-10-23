namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Underworld Breach, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Underworld Breach</exampledoc>
[TokenizationOrder(-1)]
public class UnderworldBreachRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the escape cost is equal to the card's mana cost plus exile three other cards from your graveyard"];
}
