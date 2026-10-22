namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The first half of a haunt creature's paired trigger: "this creature enters or".</summary>
/// <exampledoc>Absolver Thrull</exampledoc>
/// <examplecapture>this creature enters or</examplecapture>
[Dependent]
public class HaunterEnters : Glyph
{
    public override Nib[] Nibs => [Prop(Haunter), "enters or"];

    public ThisCreature Haunter { get; set; }
}
