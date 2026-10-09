namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A mana cost: a run of mana symbols written with nothing between them, e.g. "{2}{w}{w}".</summary>
/// <exampledoc>Conversion</exampledoc>
/// <examplecapture>{w}{w}</examplecapture>
[Dependent]
[JoinedBy(Joiner.None)]
public class ManaCost : CompoundOf<ManaSymbol>
{
}

/// <summary>One braced mana symbol: an amount of colorless mana ("{2}"), or a symbol ("{w}", "{w/u}", "{x}").</summary>
/// <exampledoc>Phantasmal Forces</exampledoc>
/// <examplecapture>{u}</examplecapture>
[Dependent]
public class ManaSymbol : GlyphOneOf
{
    public override Nib[] Nibs => ["{", Prop(Colorless), Prop(Symbol), "}"];

    public int? Colorless { get; set; }
    public ManaSymbolKind? Symbol { get; set; }
}

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
