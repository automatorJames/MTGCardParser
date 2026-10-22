namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"x more times", "six more times": a counted number of further repetitions.</summary>
/// <exampledoc>Professor Onyx</exampledoc>
/// <examplecapture>six more times</examplecapture>
[Dependent]
public class MoreTimes : Glyph
{
    public override Nib[] Nibs => [Prop(Count), "more times"];

    public Quantity Count { get; set; }
}
