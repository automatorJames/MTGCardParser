namespace MTGGlyphs.GlyphDefinitions;

/// <summary>When a forced attack or block applies, before "if able": "attacks each combat if able", "attack this turn if able", "blocks this combat if able".</summary>
public enum CombatPeriod
{
    [RegexPattern("each combat")]
    EachCombat,

    [RegexPattern("this turn")]
    ThisTurn,

    [RegexPattern("this combat")]
    ThisCombat
}
