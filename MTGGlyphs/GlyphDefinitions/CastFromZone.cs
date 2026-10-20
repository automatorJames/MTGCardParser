namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"from your graveyard", "from a graveyard", "from exile": the zone a spell is cast from.</summary>
/// <exampledoc>Burning Vengeance</exampledoc>
/// <examplecapture>from your graveyard</examplecapture>
[Dependent]
public class CastFromZone : Glyph
{
    public override Nib[] Nibs => ["from", Prop(Zone)];

    public OneOf<PlayersZone, NonBattlefieldZone?> Zone { get; set; }
}
