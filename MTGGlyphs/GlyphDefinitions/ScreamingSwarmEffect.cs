namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Screaming Swarm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Screaming Swarm</exampledoc>
[Dependent]
public class ScreamingSwarmEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put this card from your graveyard into your library second from the top"];
}
