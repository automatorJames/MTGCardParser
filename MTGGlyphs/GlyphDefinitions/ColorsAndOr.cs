namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"white and/or blue": either or both of two colors.</summary>
/// <exampledoc>Will, Scion of Peace</exampledoc>
/// <examplecapture>white and/or blue</examplecapture>
[Dependent]
public class ColorsAndOr : Glyph
{
    public override Nib[] Nibs => [Prop(First), "and/or", Prop(Second)];

    public ManaColor First { get; set; }
    public ManaColor Second { get; set; }
}
