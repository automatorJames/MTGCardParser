namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"half their life", "half your life" with no rounding stated (it's stated elsewhere: "round up each time").</summary>
/// <exampledoc>Peer into the Abyss</exampledoc>
/// <examplecapture>half their life</examplecapture>
[Dependent]
public class HalfLifeTotalExact : Glyph
{
    public override Nib[] Nibs => [Prop(Factor), Prop(Whose), "life"];

    public ScaleFactor Factor { get; set; }
    public Whose Whose { get; set; }
}
