namespace MTGGlyphs.GlyphDefinitions;

public enum Whose
{
    [RegexPattern("(an|your) opponent's")]
    Opponent,

    Your,

    [RegexPattern("its owner's")]
    ItsOwners,

    [RegexPattern("its controller's")]
    ItsControllers,

    [RegexPattern("their controllers'")]
    TheirControllers,

    Their,

    [RegexPattern("a")]
    Any,

    [RegexPattern("the next turn's")]
    NextTurns,

    TheNext,

    Each,

    The
}
