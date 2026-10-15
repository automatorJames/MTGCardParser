namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player picked out by the counters they have: "a player with ten or more poison counters".</summary>
/// <exampledoc>Marsh Viper</exampledoc>
/// <examplecapture>a player with ten or more poison counters</examplecapture>
[Dependent]
public class PlayerWithCounters : Glyph
{
    public override Nib[] Nibs => ["a player with", Prop(Count), "or more", Prop(CounterType), "counters"];

    public Quantity Count { get; set; }
    public CounterType CounterType { get; set; }
}
