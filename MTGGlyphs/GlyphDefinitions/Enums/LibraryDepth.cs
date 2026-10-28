namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A depth below the top of a library a card is put at, after "into its owner's library": "second from the top", "third from the top", "seventh from the top", or the player's pick "second from the top or on the bottom".</summary>
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
