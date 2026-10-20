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
    Twelve,

    [RegexPattern("all")]
    All,

    [RegexPattern("an additional", "one additional")]
    OneAdditional,

    [RegexPattern("any number of", "any number")]
    AnyNumber,

    [RegexPattern("that many")]
    ThatMany,

    [RegexPattern("a number of")]
    ANumberOf,

    [RegexPattern("13", "thirteen")]
    Thirteen,

    [RegexPattern("14", "fourteen")]
    Fourteen,

    [RegexPattern("15", "fifteen")]
    Fifteen,

    [RegexPattern("20", "twenty")]
    Twenty,

    [RegexPattern("[1-9][0-9]+")]
    LargerNumber
}
