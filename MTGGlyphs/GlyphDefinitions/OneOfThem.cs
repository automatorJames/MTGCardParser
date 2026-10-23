namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One of a group of objects named earlier in the line, chosen as the effect resolves: "either of them", "one of them".</summary>
/// <exampledoc>Wicked Slumber</exampledoc>
/// <examplecapture>either of them</examplecapture>
[Dependent]
public class OneOfThem : Glyph
{
    public override Nib[] Nibs => [Alt("either", "one"), "of", Prop(Group)];

    public Them Group { get; set; }
}
