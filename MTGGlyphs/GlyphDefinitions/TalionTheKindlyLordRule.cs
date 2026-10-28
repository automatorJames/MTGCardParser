namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Talion, the Kindly Lord, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Talion, the Kindly Lord</exampledoc>
[TokenizationOrder(-1)]
public class TalionTheKindlyLordRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an opponent casts a spell with mana value, power, or toughness equal to the chosen number, that player loses 2 life and you draw a card"];
}
