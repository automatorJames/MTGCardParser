namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The value chosen for X against a threshold, or an exact value, as a condition: "x is 5 or more", "x is 2 or less", "x is 3".</summary>
/// <exampledoc>Martial Coup</exampledoc>
/// <examplecapture>x is 5 or more</examplecapture>
[Dependent]
public class XThreshold : Glyph
{
    public override Nib[] Nibs => ["x is", Prop(Count), Prop(Bound)];

    public Quantity Count { get; set; }
    [Optional]
    public OrBound Bound { get; set; }
}
