namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The size of the largest group of permanents sharing a creature type: "the greatest number of creatures you control that have a creature type in common".</summary>
/// <exampledoc>Skemfar Shadowsage</exampledoc>
/// <examplecapture>the greatest number of creatures you control that have a creature type in common</examplecapture>
[Dependent]
public class LargestSharedTypeCount : Glyph
{
    public override Nib[] Nibs => ["the greatest number of", Prop(Group), "that have a creature type in common"];

    public Permanents Group { get; set; }
}
