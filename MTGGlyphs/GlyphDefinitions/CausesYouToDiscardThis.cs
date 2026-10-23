namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An opponent's effect making you discard this card or any card, as a trigger or replacement condition: "a spell or ability an opponent controls causes you to discard this card" (madness-like punishers such as Loxodon Smiter, Metrognome), "… causes you to discard a card" (Spiritual Focus), "… causes you to discard cards this turn".</summary>
/// <exampledoc>Metrognome</exampledoc>
/// <examplecapture>a spell or ability an opponent controls causes you to discard this card</examplecapture>
[Dependent]
public class CausesYouToDiscardThis : Glyph
{
    public override Nib[] Nibs => [Prop(Source), Prop(Controller), "causes you to discard", Prop(Discarded), Prop(Period)];

    public TargetingSource Source { get; set; }
    public ControlledBy Controller { get; set; }
    public DiscardedByEffect Discarded { get; set; }
    [Optional]
    public ThisPeriod Period { get; set; }
}
