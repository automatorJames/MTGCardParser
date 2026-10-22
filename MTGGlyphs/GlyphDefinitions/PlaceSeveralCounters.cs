namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Putting different counters on several permanents in one effect: "put a +1/+1 counter on each creature you control and a loyalty counter on each other planeswalker you control", "put a +1/+1 counter on target creature, two +1/+1 counters on another target creature, and three +1/+1 counters on a third target creature".</summary>
/// <exampledoc>Ajani Steadfast</exampledoc>
/// <examplecapture>put a +1/+1 counter on each creature you control and a loyalty counter on each other planeswalker you control</examplecapture>
public class PlaceSeveralCounters : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Action), Prop(Placements)];

    public CounterAction Action { get; set; }
    public ManyOf<CounterPlacement> Placements { get; set; }
}
