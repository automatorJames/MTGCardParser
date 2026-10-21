namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Condition on how the object was cast: "its prowl cost was paid", "its madness cost was paid", "its surge cost was paid".</summary>
/// <exampledoc>Latchkey Faerie</exampledoc>
/// <examplecapture>its prowl cost was paid</examplecapture>
[Dependent]
public class KeywordCostWasPaid : Glyph
{
    public override Nib[] Nibs => ["its", Prop(Keyword), "cost was paid"];

    public Keyword Keyword { get; set; }
}
