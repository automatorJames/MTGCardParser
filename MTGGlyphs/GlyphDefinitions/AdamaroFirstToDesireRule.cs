namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Adamaro, First to Desire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Adamaro, First to Desire</exampledoc>
[TokenizationOrder(-1)]
public class AdamaroFirstToDesireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["adamaro's power and toughness are each equal to the number of cards in the hand of the opponent with the most cards in hand"];
}
