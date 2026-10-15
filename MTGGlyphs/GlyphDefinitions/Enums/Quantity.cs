namespace MTGGlyphs.GlyphDefinitions;

public enum Quantity
{
    [RegexPattern("x")]
    X = -1,

    [RegexPattern("0", "zero", "none", "no")]
    Zero,

    [RegexPattern("1", "one", "an?")]
    One,

    [RegexPattern("2", "two")]
    Two,

    [RegexPattern("3", "three")]
    Three,

    [RegexPattern("4", "four")]
    Four,

    [RegexPattern("5", "five")]
    Five,

    [RegexPattern("6", "six")]
    Six,

    [RegexPattern("7", "seven")]
    Seven,

    [RegexPattern("8", "eight")]
    Eight,

    [RegexPattern("9", "nine")]
    Nine,

    [RegexPattern("10", "ten")]
    Ten,

    [RegexPattern("11", "eleven")]
    Eleven,

    [RegexPattern("12", "twelve")]
    Twelve
}
