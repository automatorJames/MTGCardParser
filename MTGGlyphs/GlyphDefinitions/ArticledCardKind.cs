namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a mount creature", "an angel", or just "a": the article and any qualifiers before "card" in a listed card kind.</summary>
/// <exampledoc>Paradox Surveyor</exampledoc>
/// <examplecapture>a</examplecapture>
[Dependent]
public class ArticledCardKind : Glyph
{
    public override Nib[] Nibs => [Pattern("an?"), Prop(Qualifiers)];

    [Optional]
    public CompoundOf<QualifierWords> Qualifiers { get; set; }
}
