namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"into your hand", "into its owner's graveyard", "into its owner's library third from the top": the zone something is put into, with an optional depth in a library.</summary>
/// <exampledoc>Bronze Tablet</exampledoc>
/// <examplecapture>into its owner's graveyard</examplecapture>
[Dependent]
public class IntoZone : Glyph
{
    public override Nib[] Nibs => ["into", Prop(Zone), Prop(Depth)];

    public PlayersZone Zone { get; set; }
    [Optional]
    public LibraryDepth? Depth { get; set; }
}
