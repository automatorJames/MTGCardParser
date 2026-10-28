namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a permanent did in combat earlier this turn, as a past-tense condition: "this creature attacked this turn", "it blocked this turn", "attacked or blocked this turn", "blocked or was blocked by a blue creature this turn".</summary>
public enum CombatHistory
{
    [RegexPattern("attacked or blocked")]
    AttackedOrBlocked,

    [RegexPattern("blocked or was blocked by")]
    BlockedOrWasBlockedBy,

    Attacked,

    Blocked
}
