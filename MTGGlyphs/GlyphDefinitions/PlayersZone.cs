namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"your hand", "its owner's hand", "your graveyard", "target opponent's hand": one player's zone.</summary>
/// <exampledoc>Unsummon</exampledoc>
/// <examplecapture>its owner's hand</examplecapture>
[Dependent]
public class PlayersZone : Glyph
{
    public override Nib[] Nibs => [Prop(Whose), Prop(Zone)];

    public OneOf<Whose?, PlayerPossessive> Whose { get; set; }
    public NonBattlefieldZone Zone { get; set; }
}
