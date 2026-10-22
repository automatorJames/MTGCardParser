namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Counters on an object or a group, of a kind or of any kind, to count: "doom counters on it", "age counter on it", "counter on it" (after "for each"), "+1/+1 counters on creatures you control".</summary>
/// <exampledoc>Armageddon Clock</exampledoc>
/// <examplecapture>doom counters on it</examplecapture>
[Dependent]
public class CountersOn : Glyph
{
    public override Nib[] Nibs => [Prop(CounterType), Plural("counter"), "on", Prop(Object)];

    [Optional]
    public CounterType? CounterType { get; set; }
    public OneOf<PermanentPhrase, Permanents> Object { get; set; }
}
