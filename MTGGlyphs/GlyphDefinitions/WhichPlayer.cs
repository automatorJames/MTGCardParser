namespace MTGGlyphs.GlyphDefinitions;

public enum WhichPlayer
{
    You,

    [RegexPattern("each opponent")]
    EachOpponent,

    [RegexPattern("an opponent")]
    AnyOpponent,

    [RegexPattern("a player")]
    AnyPlayer,

    [RegexPattern("defending player")]
    DefendingPlayer
}
