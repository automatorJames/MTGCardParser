namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Torrent Elemental, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Torrent Elemental</exampledoc>
[Dependent]
public class TorrentElementalEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put this card from exile onto the battlefield tapped"];
}
