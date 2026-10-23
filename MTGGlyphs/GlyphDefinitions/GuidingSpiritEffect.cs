namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Guiding Spirit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Guiding Spirit</exampledoc>
[Dependent]
public class GuidingSpiritEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the top card of target player's graveyard is a creature card, put that card on top of that player's library"];
}
