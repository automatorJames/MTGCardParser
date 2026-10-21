namespace MTGGlyphs.GlyphDefinitions;

public enum CombatState
{
    [RegexPattern("attacking alone")]
    AttackingAlone,

    Attacking,

    Blocking,

    Blocked,

    Unblocked
}
