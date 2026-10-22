namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A spell narrowed by a keyword or card feature it has: "that has flash", "that has convoke", "that has an adventure".</summary>
/// <exampledoc>Edgewall Innkeeper</exampledoc>
/// <examplecapture>that has an adventure</examplecapture>
[Dependent]
public class SpellHasFeature : Glyph
{
    public override Nib[] Nibs => ["that has", Prop(Feature)];

    public OneOf<Keyword?, CardFeature?> Feature { get; set; }
}
