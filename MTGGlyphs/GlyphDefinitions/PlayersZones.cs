namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Several of one player's zones, any of them a source: "that player's graveyard or hand", "your hand and graveyard".</summary>
/// <exampledoc>Psychic Intrusion</exampledoc>
/// <examplecapture>that player's graveyard or hand</examplecapture>
[Dependent]
public class PlayersZones : Glyph
{
    public override Nib[] Nibs => [Prop(Whose), Prop(Zones)];

    public Owner Whose { get; set; }
    public OneOf<ManyOf<NonBattlefieldZone>, NonBattlefieldZone?> Zones { get; set; }
}
