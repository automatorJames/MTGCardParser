namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Putting a measured number of counters on a permanent: "put a number of +1/+1 counters equal to this card's power on target creature" (scavenge), "put a number of +1/+1 counters on this creature equal to the sacrificed creature's toughness". The measure comes before or after where the counters go.</summary>
/// <exampledoc>Slitherhead</exampledoc>
/// <examplecapture>put a number of +1/+1 counters equal to this card's power on target creature</examplecapture>
public class PlaceCountersEqualTo : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Action), "a number of", Prop(Kind), "counters", Prop(CountFirst), Prop(Place), Prop(Count)];

    public CounterAction Action { get; set; }
    public OneOf<CounterType?, PowerToughnessMod> Kind { get; set; }
    [Optional]
    public EqualTo CountFirst { get; set; }
    public CounterPlace Place { get; set; }
    [Optional]
    public EqualTo Count { get; set; }
}
