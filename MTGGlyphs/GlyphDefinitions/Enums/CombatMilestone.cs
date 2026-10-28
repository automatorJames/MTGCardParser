namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A point in combat an effect is timed against: "before blockers are declared", "after attackers are declared".</summary>
public enum CombatMilestone
{
    [RegexPattern("blockers are declared")]
    BlockersDeclared,

    [RegexPattern("attackers are declared")]
    AttackersDeclared
}
