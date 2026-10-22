namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"from their hand": the zone something was moved from.</summary>
/// <exampledoc>Lost Legacy</exampledoc>
/// <examplecapture>from their hand</examplecapture>
[Dependent]
public class FromZone : Glyph
{
    public override Nib[] Nibs => ["from", Prop(Zone)];

    public PlayersZone Zone { get; set; }
}
