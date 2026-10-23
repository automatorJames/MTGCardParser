namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player event as a trigger condition, the player-side sibling of PermanentEventTrigger's events: "you gain life", "an opponent draws their second card each turn", "you commit a crime", "you win a coin flip", "you roll one or more dice", "you gain life for the first time each turn", "you lose life during your turn", "an opponent is dealt damage", "one or more opponents are dealt noncombat damage". Resolved inside TriggeredAbility's condition.</summary>
/// <exampledoc>Ajani's Pridemate</exampledoc>
/// <examplecapture>you gain life</examplecapture>
[Dependent]
public class PlayerEvent : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Prop(Event), Prop(Timing)];

    public OneOf<WhichPlayer?, PlayerGroup?> Player { get; set; }
    public PlayerEventKind Event { get; set; }
    [Optional]
    public PlayerEventTiming? Timing { get; set; }
}
