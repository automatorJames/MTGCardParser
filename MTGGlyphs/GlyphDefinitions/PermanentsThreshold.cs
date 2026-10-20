namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"two or more other lands", "two or fewer other lands", "three or more legendary creatures": at least or at most a number of permanents of a kind.</summary>
/// <exampledoc>Haunted Ridge</exampledoc>
/// <examplecapture>two or more other lands</examplecapture>
[Dependent]
public class PermanentsThreshold : Glyph
{
    public override Nib[] Nibs => [Prop(Count), "or", Prop(Bound), Prop(Other), Prop(Permanents)];

    public Quantity Count { get; set; }
    public CountBound Bound { get; set; }
    [RegexPattern("other")]
    public bool Other { get; set; }
    public Permanents Permanents { get; set; }
}
