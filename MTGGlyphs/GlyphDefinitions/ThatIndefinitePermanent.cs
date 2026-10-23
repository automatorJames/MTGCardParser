namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that a land you control", "that a gate you control": any one permanent of a kind, after "that" in a relative clause ("any type that a land you control could produce").</summary>
/// <exampledoc>Harvester Druid</exampledoc>
/// <examplecapture>that a land you control</examplecapture>
[Dependent]
public class ThatIndefinitePermanent : Glyph
{
    public override Nib[] Nibs => ["that", Prop(Permanent)];

    public IndefinitePermanent Permanent { get; set; }
}
