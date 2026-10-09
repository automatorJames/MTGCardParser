namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with three +1/+1 counters on it": counters a permanent enters the battlefield with.</summary>
/// <exampledoc>Tetravus</exampledoc>
/// <examplecapture>with three +1/+1 counters on it</examplecapture>
[Dependent]
public class EntersWithCounters : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Counters), "on it"];

    public Counters Counters { get; set; }
}
