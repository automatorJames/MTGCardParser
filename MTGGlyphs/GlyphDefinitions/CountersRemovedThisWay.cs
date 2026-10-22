namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Counters an effect or cost just removed, to count: "storage counter removed this way", "charge counter removed this way", "counter removed" (after "for each").</summary>
/// <exampledoc>Bottomless Vault</exampledoc>
/// <examplecapture>storage counter removed this way</examplecapture>
[Dependent]
public class CountersRemovedThisWay : Glyph
{
    public override Nib[] Nibs => [Prop(CounterType), Plural("counter"), "removed", Prop(ThisWay)];

    [Optional]
    public CounterType? CounterType { get; set; }
    [RegexPattern("this way")]
    public bool ThisWay { get; set; }
}
