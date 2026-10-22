namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player getting counters: "gets a poison counter", "gets two poison counters", "you get a poison counter". Either agreement, so "you" subjects take it too; a conjoined predicate, so "sacrifices a creature and gets a poison counter" does.</summary>
/// <exampledoc>Marsh Viper</exampledoc>
/// <examplecapture>gets two poison counters</examplecapture>
[Dependent]
public class GetsCounters : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("gets?"), Prop(Counters)];

    public Counters Counters { get; set; }
}
