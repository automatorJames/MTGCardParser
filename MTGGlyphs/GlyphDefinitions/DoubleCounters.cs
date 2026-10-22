namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Doubling the counters on a permanent: "double the number of +1/+1 counters on this creature", "double the number of each kind of counter on target permanent".</summary>
/// <exampledoc>Primordial Hydra</exampledoc>
/// <examplecapture>double the number of +1/+1 counters on this creature</examplecapture>
public class DoubleCounters : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["double the number of", Prop(Kind), Plural("counter"), Prop(Place)];

    public OneOf<CounterType?, EachKindOfCounter> Kind { get; set; }
    public CounterPlace Place { get; set; }
}
