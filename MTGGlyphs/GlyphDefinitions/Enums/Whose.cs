namespace MTGGlyphs.GlyphDefinitions;

public enum Whose
{
    [RegexPattern("(an|your) opponent's")]
    Opponent,

    [RegexPattern("each of your")]
    EachOfYour,

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

    [RegexPattern("a single", "a", "any")]
    Any,

    [RegexPattern("the next turn's")]
    NextTurns,

    TheNext,

    Each,

    The,

    All
}
