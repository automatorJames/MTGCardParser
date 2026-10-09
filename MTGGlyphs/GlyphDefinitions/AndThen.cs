namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"and attach {this} to it": a second effect joined onto the end of the first.</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>and attach {this} to it</examplecapture>
[Dependent]
public class AndThen : Glyph
{
    public override Nib[] Nibs => ["and", Prop(Effect)];

    public DynamicGlyph Effect { get; set; }
}
