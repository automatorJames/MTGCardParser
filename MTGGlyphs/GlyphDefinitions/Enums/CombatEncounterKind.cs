namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How a creature meets another in combat, before the other creature: "blocks a creature", "becomes blocked by a wall", "blocks or becomes blocked by a non-wall creature", "attacks with one or more creatures".</summary>
public enum CombatEncounterKind
{
    [RegexPattern("blocks or becomes blocked by")]
    BlocksOrBecomesBlockedBy,

    [RegexPattern("becomes blocked by")]
    BecomesBlockedBy,

    Blocks,

    [RegexPattern("attacks with")]
    AttacksWith
}
