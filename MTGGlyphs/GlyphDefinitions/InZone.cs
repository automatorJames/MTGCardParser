namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A zone a card is in, after "in": "in exile", "in your graveyard".</summary>
/// <exampledoc>Crackling Drake</exampledoc>
/// <examplecapture>in exile</examplecapture>
[Dependent]
public class InZone : Glyph
{
    public override Nib[] Nibs => ["in", Prop(Zone)];

    public OneOf<PlayersZone, NonBattlefieldZone?> Zone { get; set; }
}
