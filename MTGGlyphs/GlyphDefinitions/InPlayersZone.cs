namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"in your graveyard", "in your hand": where a card is, after "is".</summary>
/// <exampledoc>Bridge from Below</exampledoc>
/// <examplecapture>in your graveyard</examplecapture>
[Dependent]
public class InPlayersZone : Glyph
{
    public override Nib[] Nibs => ["in", Prop(Zone)];

    public PlayersZone Zone { get; set; }
}
