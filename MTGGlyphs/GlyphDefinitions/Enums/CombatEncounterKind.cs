namespace MTGGlyphs.GlyphDefinitions;

public enum CombatEncounterKind
{
    [RegexPattern("blocks or becomes blocked by")]
    BlocksOrBecomesBlockedBy,

    [RegexPattern("becomes blocked by")]
    BecomesBlockedBy,

    Blocks
}
