namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A card in exile, as a state: "suspended" (exiled with time counters by suspend), or "exiled", optionally with counters on it ("exiled with a scream counter on it").</summary>
/// <exampledoc>Deep-Sea Kraken</exampledoc>
/// <examplecapture>suspended</examplecapture>
[Dependent]
public class ExiledState : Glyph
{
    public override Nib[] Nibs => [Prop(State), Prop(Counters)];

    public ExileStatus State { get; set; }
    [Optional]
    public WithCountersOnIt Counters { get; set; }
}
