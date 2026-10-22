namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The value chosen for X against a threshold, as a condition: "x is 5 or more", "x is 2 or less".</summary>
/// <exampledoc>Martial Coup</exampledoc>
/// <examplecapture>x is 5 or more</examplecapture>
[Dependent]
public class XThreshold : Glyph
{
    public override Nib[] Nibs => ["x is", Prop(Count), "or", Prop(Bound)];

    public Quantity Count { get; set; }
    public CountBound Bound { get; set; }
}
