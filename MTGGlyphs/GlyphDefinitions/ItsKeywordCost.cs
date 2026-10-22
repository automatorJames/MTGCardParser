namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"its blitz cost", "its impending cost": an alternative cost named by its keyword.</summary>
/// <exampledoc>Plasma Jockey</exampledoc>
/// <examplecapture>its blitz cost</examplecapture>
[Dependent]
public class ItsKeywordCost : Glyph
{
    public override Nib[] Nibs => ["its", Prop(Keyword), "cost"];

    public Keyword Keyword { get; set; }
}
