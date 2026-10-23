namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"artifact sources", "sources", "sources of the color of your choice", "sources you don't control": a damage source named by a generic noun, narrowed by any qualifiers before it, or a chosen aspect or controller after it, rather than by a card type.</summary>
/// <exampledoc>Argothian Treefolk</exampledoc>
/// <examplecapture>artifact sources</examplecapture>
[Dependent]
public class DamageSources : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), "sources", Prop(Aspect), Prop(Controller)];

    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    [Optional]
    public ChosenAspect? Aspect { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
}
