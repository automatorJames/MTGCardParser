namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"exactly one creature", "exactly two lands": an exact number of permanents of a kind, as in a condition on how many are attacking.</summary>
/// <exampledoc>Pitfall Trap</exampledoc>
/// <examplecapture>exactly one creature</examplecapture>
[Dependent]
public class ExactPermanentCount : Glyph
{
    public override Nib[] Nibs => ["exactly", Prop(Count), Prop(Permanents)];

    public Quantity Count { get; set; }
    public Permanents Permanents { get; set; }
}
