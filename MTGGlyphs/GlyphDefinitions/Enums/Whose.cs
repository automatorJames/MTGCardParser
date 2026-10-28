namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A possessive saying whose zone, life, turn or object is meant: "your", "their", "its owner's", "its controller's", "each player's", "that player's", "an opponent's", "the chosen player's", the ordered forms "your next", "the first", "that turn's", and the articles "a", "the", "each", "all" ("your next turn", "its owner's graveyard", "each player's hand").</summary>
public enum Whose
{
    [RegexPattern("(an|your) opponent's")]
    Opponent,

    [RegexPattern("(your|all) opponents'")]
    Opponents,

    [RegexPattern("the chosen player's")]
    ChosenPlayers,

    [RegexPattern("each of your")]
    EachOfYour,

    [RegexPattern("your first")]
    YourFirst,

    [RegexPattern("your next")]
    YourNext,

    Your,

    [RegexPattern("its owner's", "his owner's", "her owner's")]
    ItsOwners,

    [RegexPattern("their owners?'s?")]
    TheirOwners,

    [RegexPattern("its controller's next")]
    ItsControllersNext,

    [RegexPattern("its controller's")]
    ItsControllers,

    [RegexPattern("their controllers'", "their controller's")]
    TheirControllers,

    [RegexPattern("their next")]
    TheirNext,

    [RegexPattern("their own")]
    TheirOwn,

    Their,

    [RegexPattern("each opponent's")]
    EachOpponents,

    [RegexPattern("each player's")]
    EachPlayers,

    [RegexPattern("that player's next")]
    ThatPlayersNext,

    [RegexPattern("that player's")]
    ThatPlayers,

    [RegexPattern("enchanted player's")]
    EnchantedPlayers,

    [RegexPattern("defending player's")]
    DefendingPlayers,

    [RegexPattern("a player's", "a single", "a", "any")]
    Any,

    [RegexPattern("the next turn's")]
    NextTurns,

    [RegexPattern("that turn's")]
    ThatTurns,

    TheNext,

    [RegexPattern("the first")]
    TheFirst,

    Each,

    The,

    All
}
