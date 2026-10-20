namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"more than one": a count strictly above a number.</summary>
/// <exampledoc>Battlefront Krushok</exampledoc>
/// <examplecapture>more than one</examplecapture>
[Dependent]
public class MoreThanCount : Glyph
{
    public override Nib[] Nibs => ["more than", Prop(Count)];

    public Quantity Count { get; set; }
}
