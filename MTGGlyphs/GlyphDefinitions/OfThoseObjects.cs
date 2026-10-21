namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"of those creatures" in "for each of those creatures": counting objects named earlier in the line.</summary>
/// <exampledoc>Archon of Absolution</exampledoc>
/// <examplecapture>of those creatures</examplecapture>
[Dependent]
public class OfThoseObjects : Glyph
{
    public override Nib[] Nibs => ["of", Prop(Objects)];

    public ThatCard Objects { get; set; }
}
