namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The top of a counter range: "-2".</summary>
/// <exampledoc>Brimstone Mage</exampledoc>
/// <examplecapture>-2</examplecapture>
[Dependent]
public class ThresholdTop : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["-", Prop(High)];

    public int High { get; set; }
}
