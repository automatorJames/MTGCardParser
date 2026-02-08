namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
[CompoundJoiner(Joiner.None)]
public class ManaValue : TokenUnitCompound
{
    [RegexPattern("[0-9]+")] public int? Colorless { get; set; }

    [RegexPattern("w")] public int? White { get; set; }
    [RegexPattern("u")] public int? Blue { get; set; }
    [RegexPattern("b")] public int? Black { get; set; }
    [RegexPattern("r")] public int? Red { get; set; }
    [RegexPattern("g")] public int? Green { get; set; }

    [RegexPattern("w/u")] public int? HybridWhiteBlue { get; set; }
    [RegexPattern("w/b")] public int? HybridWhiteBlack { get; set; }
    [RegexPattern("u/b")] public int? HybridBlueBlack { get; set; }
    [RegexPattern("u/r")] public int? HybridBlueRed { get; set; }
    [RegexPattern("b/r")] public int? HybridBlackRed { get; set; }
    [RegexPattern("b/g")] public int? HybridBlackGreen { get; set; }
    [RegexPattern("r/g")] public int? HybridRedGreen { get; set; }
    [RegexPattern("r/w")] public int? HybridRedWhite { get; set; }
    [RegexPattern("g/w")] public int? HybridGreenWhite { get; set; }
    [RegexPattern("g/u")] public int? HybridGreenBlue { get; set; }
    [RegexPattern("2/w")] public int? TwoOrWhite { get; set; }
    [RegexPattern("2/u")] public int? TwoOrBlue { get; set; }
    [RegexPattern("2/b")] public int? TwoOrBlack { get; set; }
    [RegexPattern("2/r")] public int? TwoOrRed { get; set; }
    [RegexPattern("2/g")] public int? TwoOrGreen { get; set; }

    [RegexPattern("x")] public int? X { get; set; }
    [RegexPattern("p")] public int? Phyrexian { get; set; }
    [RegexPattern("s")] public int? Snow { get; set; }
    [RegexPattern("∞")] public int? Infinite { get; set; }
}
