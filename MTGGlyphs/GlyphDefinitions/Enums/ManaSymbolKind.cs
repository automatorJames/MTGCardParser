namespace MTGGlyphs.GlyphDefinitions;

public enum ManaSymbolKind
{
    [RegexPattern("w")]
    White,

    [RegexPattern("u")]
    Blue,

    [RegexPattern("b")]
    Black,

    [RegexPattern("r")]
    Red,

    [RegexPattern("g")]
    Green,

    [RegexPattern("c")]
    Colorless,

    [RegexPattern("w/u")]
    HybridWhiteBlue,

    [RegexPattern("w/b")]
    HybridWhiteBlack,

    [RegexPattern("u/b")]
    HybridBlueBlack,

    [RegexPattern("u/r")]
    HybridBlueRed,

    [RegexPattern("b/r")]
    HybridBlackRed,

    [RegexPattern("b/g")]
    HybridBlackGreen,

    [RegexPattern("r/g")]
    HybridRedGreen,

    [RegexPattern("r/w")]
    HybridRedWhite,

    [RegexPattern("g/w")]
    HybridGreenWhite,

    [RegexPattern("g/u")]
    HybridGreenBlue,

    [RegexPattern("2/w")]
    TwoOrWhite,

    [RegexPattern("2/u")]
    TwoOrBlue,

    [RegexPattern("2/b")]
    TwoOrBlack,

    [RegexPattern("2/r")]
    TwoOrRed,

    [RegexPattern("2/g")]
    TwoOrGreen,

    [RegexPattern("x")]
    X,

    [RegexPattern("p")]
    Phyrexian,

    [RegexPattern("s")]
    Snow,

    [RegexPattern("∞")]
    Infinite
}
