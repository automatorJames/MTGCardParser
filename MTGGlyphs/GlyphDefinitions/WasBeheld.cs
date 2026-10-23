namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether the behold additional cost was paid, as a condition: "a dragon was beheld" (the Exhale spells).</summary>
/// <exampledoc>Osseous Exhale</exampledoc>
/// <examplecapture>a dragon was beheld</examplecapture>
[Dependent]
public class WasBeheld : Glyph
{
    public override Nib[] Nibs => [Prop(What), "was beheld"];

    public ArticleKind What { get; set; }
}
