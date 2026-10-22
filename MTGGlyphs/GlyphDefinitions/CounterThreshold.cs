namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A range of counter counts: "1-2" (from one to two), "4+" (four or more). Used for level up brackets and station thresholds.</summary>
/// <exampledoc>Brimstone Mage</exampledoc>
/// <examplecapture>1-2</examplecapture>
[Dependent]
public class CounterThreshold : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(Low), Prop(High)];

    public int Low { get; set; }
    public OneOf<ThresholdTop, OrMore> High { get; set; }
}
