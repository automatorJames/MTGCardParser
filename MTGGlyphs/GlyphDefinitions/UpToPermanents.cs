namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"up to seven lands", "up to two creatures you control": a limited number of permanents of a kind, not targeted.</summary>
/// <exampledoc>Great Whale</exampledoc>
/// <examplecapture>up to seven lands</examplecapture>
[Dependent]
public class UpToPermanents : Glyph
{
    public override Nib[] Nibs => ["up to", Prop(Count), Prop(Permanents)];

    public Quantity Count { get; set; }
    public Permanents Permanents { get; set; }
}
