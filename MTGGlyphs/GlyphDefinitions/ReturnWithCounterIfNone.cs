namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Persist's and undying's reminder text: "when this creature dies, if it had no -1/-1 counters on it, return it to the battlefield under its owner's control with a -1/-1 counter on it"; for a granted ability, "when it dies, …" or "when a creature with undying dies, …".</summary>
/// <exampledoc>Aerie Ouphes</exampledoc>
/// <examplecapture>when this creature dies, if it had no -1/-1 counters on it, return it to the battlefield under its owner's control with a -1/-1 counter on it</examplecapture>
public class ReturnWithCounterIfNone : Glyph
{
    public override Nib[] Nibs => ["when", Prop(Dying), "dies, if it had no", Prop(Had), "counters on it, return it to the battlefield under its owner's control with a", Prop(Gets), "counter on it"];

    public Subject Dying { get; set; }
    public CounterSize Had { get; set; }
    public CounterSize Gets { get; set; }
}
