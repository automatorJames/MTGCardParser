namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"fewer than six": a count below a number.</summary>
/// <exampledoc>Shadowborn Demon</exampledoc>
/// <examplecapture>fewer than six</examplecapture>
[Dependent]
public class FewerThanCount : Glyph
{
    public override Nib[] Nibs => ["fewer than", Prop(Count)];

    public Quantity Count { get; set; }
}
