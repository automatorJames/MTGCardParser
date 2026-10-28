namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A creature's state in the current combat, as a qualifier or predicate: "attacking", "blocking", "blocked", "unblocked", "attacking alone", as in "attacking creature", "the blocking creature", "isn't attacking".</summary>
public enum CombatState
{
    [RegexPattern("attacking alone")]
    AttackingAlone,

    Attacking,

    Blocking,

    Blocked,

    Unblocked
}
