namespace MTGGlyphs.GlyphDefinitions;

public enum WhichPlayer
{
    [RegexPattern("your opponents")]
    YourOpponents,

    You,

    [RegexPattern("each opponent")]
    EachOpponent,

    [RegexPattern("each other player")]
    EachOtherPlayer,

    [RegexPattern("an opponent of your choice")]
    OpponentOfYourChoice,

    [RegexPattern("an opponent")]
    AnyOpponent,

    [RegexPattern("a player", "any player")]
    AnyPlayer,

    [RegexPattern("players")]
    Players,

    [RegexPattern("defending player")]
    DefendingPlayer
}
