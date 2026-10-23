namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Endless Whispers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Endless Whispers</exampledoc>
[TokenizationOrder(-1)]
public class EndlessWhispersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player puts this card from its owner's graveyard onto the battlefield under their control at the beginning of the next end step"];
}
