namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Counters of a named kind, with no number: "+1/+1 counters", "loyalty counters".</summary>
/// <exampledoc>Hardened Scales</exampledoc>
/// <examplecapture>+1/+1 counters</examplecapture>
[Dependent]
public class KindOfCounters : Glyph
{
    public override Nib[] Nibs => [Prop(Kind), "counters"];

    public CounterType Kind { get; set; }
}
