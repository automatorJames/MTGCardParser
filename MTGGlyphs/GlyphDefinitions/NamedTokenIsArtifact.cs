namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A predefined token named in its reminder: "a treasure token is an artifact with "".</summary>
/// <exampledoc>Mine Raider</exampledoc>
/// <examplecapture>a treasure token is an artifact with "</examplecapture>
[Dependent]
public class NamedTokenIsArtifact : Glyph
{
    public override Nib[] Nibs => ["a", Prop(Kind), "token is an artifact with \""];

    public PredefinedToken Kind { get; set; }
}
