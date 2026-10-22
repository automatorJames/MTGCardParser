namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"cards in your graveyard": the cards in one player's zone.</summary>
/// <exampledoc>Gnarlwood Dryad</exampledoc>
/// <examplecapture>cards in your graveyard</examplecapture>
[Dependent]
public class CardsInPlayersZone : Glyph
{
    public override Nib[] Nibs => ["cards in", Prop(Zone)];

    public PlayersZone Zone { get; set; }
}
