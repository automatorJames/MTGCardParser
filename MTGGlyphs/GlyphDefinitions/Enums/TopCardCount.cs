namespace MTGGlyphs.GlyphDefinitions;

public enum TopCardCount
{
    [RegexPattern("that many")]
    ThatMany,

    [RegexPattern("a number of")]
    ANumberOf,

    [RegexPattern("twice x")]
    TwiceX
}
