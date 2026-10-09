namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A state trigger on a player controlling no permanents of a kind: "when you control no islands, sacrifice {this}".</summary>
/// <exampledoc>Dandân</exampledoc>
/// <examplecapture>when you control no islands, sacrifice {this}</examplecapture>
public class ControlsNoneTrigger : Glyph
{
    public override Nib[] Nibs => ["when", Prop(Controller), Pattern("controls?"), "no", Plural(Prop(Kind)), ",", Prop(Effect)];

    public WhichPlayer Controller { get; set; }
    public PermanentKind Kind { get; set; }
    [AllowUnmatched]
    public DynamicGlyph Effect { get; set; }
}
