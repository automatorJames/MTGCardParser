namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Putting counters on, or removing them from, a permanent: "put a +1/+1 counter on target creature", "remove a doom counter from {this}", "remove a time counter" (the permanent is implied), "put your choice of a reach counter, a vigilance counter, or a trample counter on it", or as what a subject does, "(that player) puts an age counter on it". Also a cost: "remove a +1/+1 counter from {this}: …".</summary>
/// <exampledoc>Armageddon Clock</exampledoc>
/// <examplecapture>put a doom counter on {this}</examplecapture>
public class PlaceCounters : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Action), Prop(Counters), Prop(Place)];

    public CounterAction Action { get; set; }
    public OneOf<Counters, ChoiceOfCounters> Counters { get; set; }
    [Optional]
    public CounterPlace Place { get; set; }
}
