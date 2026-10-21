namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The controller or owner of a target, as a subject: "target creature's owner", "target permanent's controller".</summary>
/// <exampledoc>Dire Downdraft</exampledoc>
/// <examplecapture>target creature's owner</examplecapture>
[Dependent]
public class TargetsController : Glyph
{
    public override Nib[] Nibs => [Prop(Target), "'s", Prop(Role)];

    public SpecificTarget Target { get; set; }
    public PlayerRole Role { get; set; }
}
