namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ichorid, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ichorid</exampledoc>
/// <examplecapture>if this card is in your graveyard, you may exile a black creature card other than this card from your graveyard</examplecapture>
[Dependent]
public class IchoridEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this card is in your graveyard, you may exile a black creature card other than this card from your graveyard"];
}
