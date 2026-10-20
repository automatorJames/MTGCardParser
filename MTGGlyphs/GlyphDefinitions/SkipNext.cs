namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Skipping a step or turn: "skip your next turn", "skips their next draw step".</summary>
/// <exampledoc>Chronatog</exampledoc>
/// <examplecapture>skip your next turn</examplecapture>
[Dependent]
public class SkipNext : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Step)];

    public SkipVerb Verb { get; set; }
    public NextPlayersPhase Step { get; set; }
}
