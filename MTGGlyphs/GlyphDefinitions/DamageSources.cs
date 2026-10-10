namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"artifact sources", "sources": a damage source named by a generic noun, narrowed by any qualifiers, rather than by a card type.</summary>
/// <exampledoc>Argothian Treefolk</exampledoc>
/// <examplecapture>artifact sources</examplecapture>
[Dependent]
public class DamageSources : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), "sources"];

    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
}
