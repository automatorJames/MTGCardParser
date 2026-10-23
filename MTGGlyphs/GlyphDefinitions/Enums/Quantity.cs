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

    [RegexPattern("one or more")]
    OneOrMore,

    [RegexPattern("one or two")]
    OneOrTwo,

    [RegexPattern("that many plus one")]
    ThatManyPlusOne,

    [RegexPattern("twice that many")]
    TwiceThatMany,

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
    LargerNumber,

    [RegexPattern("x plus 1", "x plus one")]
    XPlusOne,

    [RegexPattern("x plus 2", "x plus two")]
    XPlusTwo,

    [RegexPattern("x plus 3", "x plus three")]
    XPlusThree,

    [RegexPattern("x minus 1", "x minus one")]
    XMinusOne,

    [RegexPattern("twice x")]
    TwiceX
}
