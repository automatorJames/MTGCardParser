namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A colored predefined token's reminder opening: "it's a red and green artifact with "", "they're red and green artifacts with "" (Heartwood).</summary>
/// <exampledoc>Hungering Puppetbeast</exampledoc>
/// <examplecapture>it's a red and green artifact with "</examplecapture>
[Dependent]
public class ColoredTokenIsArtifact : Glyph
{
    public override Nib[] Nibs => [Alt("it's a", "they're"), Prop(Colors), Plural("artifact"), "with \""];

    public OneOf<ManyOf<ManaColor>, ManaColor?> Colors { get; set; }
}
