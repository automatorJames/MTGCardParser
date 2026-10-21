namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Condition that an optional or alternative cost was paid: "its prowl cost was paid", "this spell's madness cost was paid", "this spell's additional cost was paid", "this creature's spectacle cost was paid".</summary>
/// <exampledoc>Latchkey Faerie</exampledoc>
/// <examplecapture>its prowl cost was paid</examplecapture>
[Dependent]
public class KeywordCostWasPaid : Glyph
{
    public override Nib[] Nibs => [Prop(Whose), Prop(Additional), Prop(Keyword), "cost was paid"];

    public OneOf<Its, ObjectPossessive> Whose { get; set; }
    [RegexPattern("additional")]
    public bool Additional { get; set; }
    [Optional]
    public Keyword? Keyword { get; set; }
}
