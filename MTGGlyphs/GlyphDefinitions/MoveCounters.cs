namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Moving counters from one permanent onto another: "move a +1/+1 counter from this creature onto it" (graft), "move any number of +1/+1 counters from target creature onto this creature", "move all +1/+1 counters from all creatures onto it".</summary>
/// <exampledoc>Simic Initiate</exampledoc>
/// <examplecapture>move a +1/+1 counter from this creature onto it</examplecapture>
public class MoveCounters : Glyph, IPredicate
{
    public override Nib[] Nibs => ["move", Prop(Counters), "from", Prop(Source), "onto", Prop(Destination)];

    public Counters Counters { get; set; }
    public CounterRecipient Source { get; set; }
    public CounterRecipient Destination { get; set; }
}
