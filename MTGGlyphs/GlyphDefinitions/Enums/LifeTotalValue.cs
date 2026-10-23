namespace MTGGlyphs.GlyphDefinitions;

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
