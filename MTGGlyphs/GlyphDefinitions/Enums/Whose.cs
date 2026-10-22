namespace MTGGlyphs.GlyphDefinitions;

public enum Whose
{
    [RegexPattern("(an|your) opponent's")]
    Opponent,

    [RegexPattern("each of your")]
    EachOfYour,

    [RegexPattern("your first")]
    YourFirst,

    [RegexPattern("your next")]
    YourNext,

    Your,

    [RegexPattern("its owner's")]
    ItsOwners,

    [RegexPattern("their owners?'s?")]
    TheirOwners,

    [RegexPattern("its controller's")]
    ItsControllers,

    [RegexPattern("their controllers'", "their controller's")]
    TheirControllers,

    Their,

    [RegexPattern("each opponent's")]
    EachOpponents,

    [RegexPattern("each player's")]
    EachPlayers,

    [RegexPattern("that player's")]
    ThatPlayers,

    [RegexPattern("enchanted player's")]
    EnchantedPlayers,

    [RegexPattern("a single", "a", "any")]
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
