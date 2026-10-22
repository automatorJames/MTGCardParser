namespace MTGGlyphs.GlyphDefinitions;

public enum LibraryDepth
{
    [RegexPattern("second from the top or on the bottom")]
    SecondFromTheTopOrBottom,

    [RegexPattern("second from the top")]
    Second,

    [RegexPattern("third from the top")]
    Third,

    [RegexPattern("fourth from the top")]
    Fourth,

    [RegexPattern("fifth from the top")]
    Fifth,

    [RegexPattern("sixth from the top")]
    Sixth,

    [RegexPattern("seventh from the top")]
    Seventh
}
