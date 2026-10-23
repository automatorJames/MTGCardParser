namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One counter singled out by its place in the count: "the fourth plan counter", "the twelfth hour counter" - the threshold counter that triggers a plan or a clock.</summary>
/// <exampledoc>Rewrite History</exampledoc>
/// <examplecapture>the fourth plan counter</examplecapture>
[Dependent]
public class NthCounter : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Ordinal), Prop(Kind), "counter"];

    public SpellOrdinal Ordinal { get; set; }
    public CounterType Kind { get; set; }
}
