namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"up to two": the most of something an effect may choose, short of a fixed number.</summary>
/// <exampledoc>Gravegouger</exampledoc>
/// <examplecapture>up to two</examplecapture>
[Dependent]
public class UpToCount : Glyph
{
    public override Nib[] Nibs => ["up to", Prop(Count)];

    public Quantity Count { get; set; }
}
