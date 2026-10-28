namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The span of another player's turn a Mindslaver effect takes over, after "next": "control target player during that player's next turn", "control target opponent during their next combat phase".</summary>
public enum ControlledPeriod
{
    Turn,

    [RegexPattern("combat phase")]
    CombatPhase
}
