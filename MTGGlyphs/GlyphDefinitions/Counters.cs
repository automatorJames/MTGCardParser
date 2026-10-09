namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A number of counters of a kind: "a doom counter", "three +1/+1 counters".</summary>
/// <exampledoc>Tetravus</exampledoc>
/// <examplecapture>three +1/+1 counters</examplecapture>
[Dependent]
public class Counters : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), Prop(CounterType), Plural("counter")];

    public Quantity Quantity { get; set; }
    public CounterType CounterType { get; set; }
}
