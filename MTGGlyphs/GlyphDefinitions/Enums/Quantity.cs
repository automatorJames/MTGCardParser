namespace MTGGlyphs.GlyphDefinitions;

public enum Quantity
{
    [RegexPattern("x")]
    X = -1,

    [RegexPattern("0", "zero", "none")]
    Zero = 0,

    [RegexPattern("1", "one", "a")]
    One = 1,

    [RegexPattern("2", "two")]
    Two = 2,

    [RegexPattern("3", "three")]
    Three = 3,

    [RegexPattern("4", "four")]
    Four = 4,

    [RegexPattern("5", "five")]
    Five = 5,

    [RegexPattern("6", "six")]
    Six = 6,

    [RegexPattern("7", "seven")]
    Seven = 7,

    [RegexPattern("8", "eight")]
    Eight = 8,

    [RegexPattern("9", "nine")]
    Nine = 9,

    [RegexPattern("10", "ten")]
    Ten = 10,

    [RegexPattern("11", "eleven")]
    Eleven = 11,

    [RegexPattern("12", "twelve")]
    Twelve = 12
}
