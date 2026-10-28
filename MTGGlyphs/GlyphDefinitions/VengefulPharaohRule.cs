namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vengeful Pharaoh, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vengeful Pharaoh</exampledoc>
[TokenizationOrder(-1)]
public class VengefulPharaohRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever combat damage is dealt to you or a planeswalker you control, if this card is in your graveyard, destroy target attacking creature, then put this card on top of your library"];
}
