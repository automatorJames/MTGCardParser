namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nether Spirit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nether Spirit</exampledoc>
[Dependent]
public class NetherSpiritEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this card is the only creature card in your graveyard, you may return this card to the battlefield"];
}
