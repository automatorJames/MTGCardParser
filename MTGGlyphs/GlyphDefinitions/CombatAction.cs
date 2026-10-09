namespace MTGGlyphs.GlyphDefinitions;

public enum CombatAction
{
    Attack,

    Block,

    [RegexPattern("be blocked")]
    BeBlocked
}
