namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"your hand", "its owner's hand", "your graveyard": one player's zone.</summary>
/// <exampledoc>Unsummon</exampledoc>
/// <examplecapture>its owner's hand</examplecapture>
[Dependent]
public class PlayersZone : Glyph
{
    public override Nib[] Nibs => [Prop(Whose), Prop(Zone)];

    public Whose Whose { get; set; }
    public NonBattlefieldZone Zone { get; set; }
}
