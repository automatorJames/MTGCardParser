namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"from white sources", "from artifact sources": abilities narrowed by their source's color or type.</summary>
/// <exampledoc>Rebel Informer</exampledoc>
/// <examplecapture>from white sources</examplecapture>
[Dependent]
public class FromSources : Glyph
{
    public override Nib[] Nibs => ["from", Prop(Qualifier), "sources"];

    public TargetQualifier Qualifier { get; set; }
}
