namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A basic land type a land becomes: "an island", "a forest", "swamps".</summary>
/// <exampledoc>Streambed Aquitects</exampledoc>
/// <examplecapture>an island</examplecapture>
[Dependent]
public class BecomesLandType : Glyph
{
    public override Nib[] Nibs => [Prop(Article), Plural(Prop(LandType))];

    [RegexPattern("an?")]
    public bool Article { get; set; }
    public LandType LandType { get; set; }
}
