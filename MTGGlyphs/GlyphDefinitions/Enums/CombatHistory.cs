namespace MTGGlyphs.GlyphDefinitions;

public enum CombatHistory
{
    [RegexPattern("attacked or blocked")]
    AttackedOrBlocked,

    [RegexPattern("blocked or was blocked by")]
    BlockedOrWasBlockedBy,

    Attacked,

    Blocked
}
