namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What picks out players after "who": controlling a permanent of a kind, having a number of poison counters or a hand size, having lost or gained life this turn, having done something else this turn ("cast a spell this turn"), having searched this way, or having done (or not) what was just offered.</summary>
/// <exampledoc>Hired Giant</exampledoc>
/// <examplecapture>searched their library this way</examplecapture>
[Dependent]
public class PlayerRelative : GlyphOneOf
{
    public ControlsAPermanent Controls { get; set; }
    public HasPoisonCounters Poison { get; set; }
    public HoldsHandSize HandSize { get; set; }
    public LifeChangedThisTurn LifeChanged { get; set; }
    public PastActionThisTurn PastAction { get; set; }
    public SearchedThisWay Searched { get; set; }
    public PlayerResponse? Response { get; set; }
}
