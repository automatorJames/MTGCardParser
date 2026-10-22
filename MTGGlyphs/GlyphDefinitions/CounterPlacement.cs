namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One counter placement in a list of several: "a +1/+1 counter on target creature", "a loyalty counter on each other planeswalker you control".</summary>
/// <exampledoc>Ajani Steadfast</exampledoc>
/// <examplecapture>a loyalty counter on each other planeswalker you control</examplecapture>
[Dependent]
public class CounterPlacement : Glyph
{
    public override Nib[] Nibs => [Prop(Counters), Prop(Place)];

    public Counters Counters { get; set; }
    public CounterPlace Place { get; set; }
}
