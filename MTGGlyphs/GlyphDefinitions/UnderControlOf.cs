namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"under your control": who controls a permanent that is put onto the battlefield.</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>under your control</examplecapture>
[Dependent]
public class UnderControlOf : Glyph
{
    public override Nib[] Nibs => ["under", Prop(Whose), "control"];

    public Whose Whose { get; set; }
}
