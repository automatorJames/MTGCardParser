namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A count of cards off the top of a library, before "cards from the top of": "that many" ("that many cards from the top of your library"), "a number of" (with "equal to …" after the library), or "twice x".</summary>
public enum TopCardCount
{
    [RegexPattern("that many")]
    ThatMany,

    [RegexPattern("a number of")]
    ANumberOf,

    [RegexPattern("twice x")]
    TwiceX
}
