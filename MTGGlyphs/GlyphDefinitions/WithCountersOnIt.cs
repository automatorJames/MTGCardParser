namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with a +1/+1 counter on it", "with a finality counter on it", "with three or more level counters on it", "with +1/+1 counters on them", "with counters on them": permanents carrying counters.</summary>
/// <exampledoc>Proctor of Potential</exampledoc>
/// <examplecapture>with a finality counter on it</examplecapture>
[Dependent]
public class WithCountersOnIt : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Counters), "on", Alt("it", "them")];

    public OneOf<Counters, CountersOfKind> Counters { get; set; }
}
