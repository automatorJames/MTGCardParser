namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A single source of damage, as replacement and prevention effects name one: "a source", "a red source", "a source an opponent controls", "another source you control".</summary>
/// <exampledoc>Guardian Seraph</exampledoc>
/// <examplecapture>a source an opponent controls</examplecapture>
[Dependent]
public class ASource : Glyph
{
    public override Nib[] Nibs => [Alt("a", "another"), Prop(Qualifiers), "source", Prop(Controller)];

    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
}
