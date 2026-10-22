namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"4 or greater", "2 or less": a number and the direction it bounds.</summary>
/// <exampledoc>Bloodshot Trainee</exampledoc>
/// <examplecapture>4 or greater</examplecapture>
[Dependent]
public class NumericBound : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), "or", Prop(Comparison)];

    public Quantity Quantity { get; set; }
    public Comparison Comparison { get; set; }
}
