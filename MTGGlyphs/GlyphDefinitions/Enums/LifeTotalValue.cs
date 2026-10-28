namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A stock life-total value used as an amount: "your starting life total", "the highest life total among players", "the lowest life total among all players", "the highest life total among your opponents", or "that number" ("your life total becomes that number").</summary>
public enum LifeTotalValue
{
    [RegexPattern("equal to your starting life total", "your starting life total")]
    StartingLifeTotal,

    [RegexPattern("the highest life total among (?:all )?players")]
    HighestAmongPlayers,

    [RegexPattern("the lowest life total among (?:all )?players")]
    LowestAmongPlayers,

    [RegexPattern("the highest life total among your opponents")]
    HighestAmongOpponents,

    [RegexPattern("that number")]
    ThatNumber
}
