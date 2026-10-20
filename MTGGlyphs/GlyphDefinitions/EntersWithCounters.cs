namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with three +1/+1 counters on it", "with a +1/+1 counter on it for each creature card in your graveyard": counters a permanent enters the battlefield with, optionally multiplied.</summary>
/// <exampledoc>Tetravus</exampledoc>
/// <examplecapture>with three +1/+1 counters on it</examplecapture>
[Dependent]
public class EntersWithCounters : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Counters), "on it", Prop(PerEach)];

    public Counters Counters { get; set; }
    [Optional]
    public ForEach PerEach { get; set; }
}
