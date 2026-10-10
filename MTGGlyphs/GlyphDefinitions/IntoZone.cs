namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"into your hand", "into its owner's graveyard": the zone something is put into.</summary>
/// <exampledoc>Bronze Tablet</exampledoc>
/// <examplecapture>into its owner's graveyard</examplecapture>
[Dependent]
public class IntoZone : Glyph
{
    public override Nib[] Nibs => ["into", Prop(Zone)];

    public PlayersZone Zone { get; set; }
}
