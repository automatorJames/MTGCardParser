namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with a +1/+1 counter on it", "with a finality counter on it", "with three or more level counters on it": permanents carrying counters.</summary>
/// <exampledoc>Proctor of Potential</exampledoc>
/// <examplecapture>with a finality counter on it</examplecapture>
[Dependent]
public class WithCountersOnIt : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Counters), "on it"];

    public Counters Counters { get; set; }
}
