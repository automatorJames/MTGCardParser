namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player controlling a particular permanent: "you control {this}".</summary>
/// <exampledoc>Aladdin</exampledoc>
/// <examplecapture>you control {this}</examplecapture>
[Dependent]
public class ControlsPermanent : Glyph
{
    public override Nib[] Nibs => [Prop(Controller), Pattern("controls?"), Prop(Permanent)];

    public WhichPlayer Controller { get; set; }
    public PermanentPhrase Permanent { get; set; }
}
