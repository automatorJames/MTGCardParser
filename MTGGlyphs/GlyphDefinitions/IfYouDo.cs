namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"if you do, …": what follows a choice you made, such as paying an optional cost.</summary>
/// <exampledoc>Crystal Rod</exampledoc>
/// <examplecapture>if you do, you gain 1 life</examplecapture>
public class IfYouDo : Glyph
{
    public override Nib[] Nibs => ["if you do, ", Prop(Outcome)];

    public DynamicGlyph Outcome { get; set; }
}