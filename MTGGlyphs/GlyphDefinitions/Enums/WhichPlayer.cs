namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player or set of players named in rules text: "you", "each opponent", "each player", "an opponent", "any player", "no player", "defending player", "attacking player", "enchanted player"; the subject of player predicates and the player in conditions, triggers and comparisons.</summary>
public enum WhichPlayer
{
    [RegexPattern("your opponents")]
    YourOpponents,

    You,

    [RegexPattern("each opponent", "each of your opponents")]
    EachOpponent,

    [RegexPattern("each of that player's opponents")]
    EachOfThatPlayersOpponents,

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
