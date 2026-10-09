namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with flying", "without flying", "with flying or reach": a qualifier on permanents having, or lacking, one of some keywords.</summary>
/// <exampledoc>Dancing Scimitar</exampledoc>
/// <examplecapture>with flying or reach</examplecapture>
[Dependent]
public class WithKeywords : Glyph
{
    public override Nib[] Nibs => [Prop(Having), Prop(Keywords)];

    public Having Having { get; set; }
    public OneOf<ManyOf<Keyword>, Keyword?> Keywords { get; set; }
}
