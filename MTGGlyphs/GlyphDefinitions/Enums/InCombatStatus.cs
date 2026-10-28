namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A creature's part in combat: "attacking", "blocking", "attacking or blocking", in "that's attacking or blocking", "each creature it's blocking".</summary>
public enum InCombatStatus
{
    [RegexPattern("attacking or blocking")]
    AttackingOrBlocking,

    Attacking,

    Blocking
}
