namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player event as a trigger condition, the player-side sibling of PermanentEventTrigger's events: "you gain life", "an opponent draws their second card each turn", "you commit a crime". Resolved inside TriggeredAbility's condition.</summary>
/// <exampledoc>Ajani's Pridemate</exampledoc>
/// <examplecapture>you gain life</examplecapture>
[Dependent]
public class PlayerEvent : Glyph
{
    public WhichPlayer Player { get; set; }
    public PlayerEventKind Event { get; set; }
}
