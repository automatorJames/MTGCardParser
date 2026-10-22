namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A triggered ability that exists only for a time, its duration first: "until end of turn, whenever a player taps an island for mana, that player adds an additional {u}", "until end of turn, whenever target creature deals damage, you gain that much life".</summary>
/// <exampledoc>High Tide</exampledoc>
/// <examplecapture>until end of turn, whenever a player taps an island for mana, that player adds an additional {u}</examplecapture>
public class DurationScopedTrigger : Glyph
{
    public override Nib[] Nibs => [Prop(Duration), ",", Prop(Trigger)];

    public OneOf<UntilPhase, UntilEndOfPlayersTurn, DuringTurn> Duration { get; set; }
    public OneOf<PermanentEventTrigger, TapForManaTrigger> Trigger { get; set; }
}
