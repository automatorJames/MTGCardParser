namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A replacement effect that increases counters as they're put on permanents: "if one or more +1/+1 counters would be put on a creature you control, twice that many +1/+1 counters are put on it instead", "… that many plus one of each of those kinds of counters are put on that permanent instead" (Hardened Scales, Corpsejack Menace, Winding Constrictor).</summary>
/// <exampledoc>Hardened Scales</exampledoc>
/// <examplecapture>if one or more +1/+1 counters would be put on a creature you control, that many plus one +1/+1 counters are put on it instead</examplecapture>
public class CounterIncreaseReplacement : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Counters), "would be put on", Prop(Recipient), ",", Prop(Increase), Prop(Kinds), "are put on", Prop(Again), "instead"];

    public Counters Counters { get; set; }
    public CounterRecipient Recipient { get; set; }
    public CounterIncrease Increase { get; set; }
    public OneOf<CountersOfKind, EachOfThoseKindsOfCounters> Kinds { get; set; }
    public CounterRecipient Again { get; set; }
}
