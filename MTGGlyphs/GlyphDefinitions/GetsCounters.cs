namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player getting counters: "gets a poison counter", "gets two poison counters".</summary>
/// <exampledoc>Marsh Viper</exampledoc>
/// <examplecapture>gets two poison counters</examplecapture>
[Dependent]
public class GetsCounters : Glyph, IPredicate
{
    public override Nib[] Nibs => ["gets", Prop(Counters)];

    public Counters Counters { get; set; }
}
