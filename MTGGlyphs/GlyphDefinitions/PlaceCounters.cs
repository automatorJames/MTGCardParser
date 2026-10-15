namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Putting counters on, or removing them from, a permanent: "put a +1/+1 counter on target creature", "remove a doom counter from {this}", or as what a subject does, "(that player) puts an age counter on it". Also a cost: "remove a +1/+1 counter from {this}: …".</summary>
/// <exampledoc>Armageddon Clock</exampledoc>
/// <examplecapture>put a doom counter on {this}</examplecapture>
public class PlaceCounters : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Action), Prop(Counters), Alt("on", "from"), Prop(Permanent)];

    public CounterAction Action { get; set; }
    public Counters Counters { get; set; }
    public PermanentPhrase Permanent { get; set; }
}
