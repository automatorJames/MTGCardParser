namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"target land", "target creature", "target permanent you own": a bare target of one kind, as what auras or equipment are attached to - kept small so it can sit inside a group of permanents.</summary>
/// <exampledoc>Street Sweeper</exampledoc>
/// <examplecapture>target land</examplecapture>
[Dependent]
[Referent]
public class AttachmentTarget : Glyph
{
    public override Nib[] Nibs => ["target", Prop(Kind), Prop(Controller)];

    public TargetKind Kind { get; set; }
    [Optional]
    public PlainControlledBy Controller { get; set; }
}
