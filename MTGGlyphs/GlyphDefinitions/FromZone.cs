namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"from your graveyard", "from their hand", "from a graveyard", "from exile": the zone something is moved or cast from.</summary>
/// <exampledoc>Lost Legacy</exampledoc>
/// <examplecapture>from their hand</examplecapture>
[Dependent]
public class FromZone : Glyph
{
    public override Nib[] Nibs => ["from", Prop(Zone)];

    public OneOf<PlayersZone, NonBattlefieldZone?> Zone { get; set; }
}
