namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The source damage would come from: "that would be dealt by this creature", "that would be dealt by creatures you control".</summary>
/// <exampledoc>Excruciator</exampledoc>
/// <examplecapture>that would be dealt by this creature</examplecapture>
[Dependent]
public class WouldBeDealtBy : Glyph
{
    public override Nib[] Nibs => ["that would be dealt by", Prop(Source)];

    public Subject Source { get; set; }
}
