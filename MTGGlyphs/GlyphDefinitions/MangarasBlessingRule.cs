namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mangara's Blessing, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mangara's Blessing</exampledoc>
[TokenizationOrder(-1)]
public class MangarasBlessingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when a spell or ability an opponent controls causes you to discard this card, you gain 2 life, and you return this card from your graveyard to your hand at the beginning of the next end step"];
}
