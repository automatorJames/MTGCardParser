namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Skipping a step or turn: "skip your next turn", "skips their next draw step", "skip your draw step", "players skip their upkeep steps", "skip your draw step this turn", "skip your next two turns", or an event about to happen, "skip that draw", "skips that turn". Also an imperative on its own ("skip your draw step").</summary>
/// <exampledoc>Chronatog</exampledoc>
/// <examplecapture>skip your next turn</examplecapture>
public class SkipNext : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Step), Prop(Turn)];

    public SkipVerb Verb { get; set; }
    public OneOf<NextPlayersPhase, PlayersPhase, OneOf<NextTurns, SkippedEvent?>> Step { get; set; }
    [Optional]
    public TurnSpan? Turn { get; set; }
}
