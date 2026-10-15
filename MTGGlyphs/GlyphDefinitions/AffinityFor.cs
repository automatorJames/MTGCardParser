namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Affinity for a kind of permanent: "affinity for artifacts".</summary>
/// <exampledoc>Assert Authority</exampledoc>
/// <examplecapture>affinity for artifacts</examplecapture>
[Dependent]
public class AffinityFor : Glyph
{
    public override Nib[] Nibs => ["affinity for", Plural(Prop(Kind))];

    public PermanentKind Kind { get; set; }
}
