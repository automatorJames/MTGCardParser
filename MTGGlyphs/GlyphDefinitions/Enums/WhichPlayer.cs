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

    [RegexPattern("each player")]
    EachPlayer,

    [RegexPattern("an opponent of your choice")]
    OpponentOfYourChoice,

    [RegexPattern("an opponent", "any opponent")]
    AnyOpponent,

    [RegexPattern("no opponent")]
    NoOpponent,

    [RegexPattern("no player", "no one")]
    NoPlayer,

    [RegexPattern("a player", "any player")]
    AnyPlayer,

    [RegexPattern("players")]
    Players,

    [RegexPattern("defending player")]
    DefendingPlayer,

    [RegexPattern("attacking player", "the attacking player")]
    AttackingPlayer,

    [RegexPattern("enchanted player")]
    EnchantedPlayer
}
