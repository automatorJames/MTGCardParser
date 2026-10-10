namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Counters of a kind on an object, to count: "doom counters on it".</summary>
/// <exampledoc>Armageddon Clock</exampledoc>
/// <examplecapture>doom counters on it</examplecapture>
[Dependent]
public class CountersOn : Glyph
{
    public override Nib[] Nibs => [Prop(CounterType), "counters on", Prop(Object)];

    public CounterType CounterType { get; set; }
    public PermanentPhrase Object { get; set; }
}
