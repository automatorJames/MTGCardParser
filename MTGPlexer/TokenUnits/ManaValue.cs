namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
public class ManaValue : TokenUnitDistilled
{
    protected override Snippet[] Snippets => [Prop(ManaSymbols)];


    [RegexPattern(@"(\{([0-9]+|[wubrgxyzc∞]|w/u|w/b|u/b|u/r|b/r|b/g|r/g|r/w|g/w|g/u|2/w|2/u|2/b|2/r|2/g|p|s)\})+")]
    public PlaceholderCapture ManaSymbols { get; set; }

    public override void DistillValuesFromPlaceholders()
    {
        throw new Exception($"fixy thisy latery");

        //// local helper
        //int? Increment(int? v, int by = 1) 
        //    => (v ?? 0) + by;
        //
        ////var matches = TokenTypeRegistry.Templates[Type].Regex.Matches(Match.RootMatch.Value);
        //
        //foreach (Match match in matches)
        //{
        //    var symbols = match.Groups[nameof(ManaSymbols)].Value.ToLowerInvariant();
        //    var symbolsMatches = Regex.Matches(symbols, @"(?<=\{)[^{}]+(?=\})");
        //
        //    foreach (var symbol in symbolsMatches.Select(x => x.Value))
        //    {
        //        switch (symbol)
        //        {
        //            case "w": White = Increment(White); break;
        //            case "u": Blue = Increment(Blue); break;
        //            case "b": Black = Increment(Black); break;
        //            case "r": Red = Increment(Red); break;
        //            case "g": Green = Increment(Green); break;
        //            case "x": X = Increment(X); break;
        //            case "c": Colorless = Increment(Colorless); break;
        //            case "∞": Infinite = Increment(Infinite); break;
        //            case "p": Phyrexian = Increment(Phyrexian); break;
        //            case "s": Snow = Increment(Snow); break;
        //
        //            case "w/u": HybridWhiteBlue = Increment(HybridWhiteBlue); break;
        //            case "w/b": HybridWhiteBlack = Increment(HybridWhiteBlack); break;
        //            case "u/b": HybridBlueBlack = Increment(HybridBlueBlack); break;
        //            case "u/r": HybridBlueRed = Increment(HybridBlueRed); break;
        //            case "b/r": HybridBlackRed = Increment(HybridBlackRed); break;
        //            case "b/g": HybridBlackGreen = Increment(HybridBlackGreen); break;
        //            case "r/g": HybridRedGreen = Increment(HybridRedGreen); break;
        //            case "r/w": HybridRedWhite = Increment(HybridRedWhite); break;
        //            case "g/w": HybridGreenWhite = Increment(HybridGreenWhite); break;
        //            case "g/u": HybridGreenBlue = Increment(HybridGreenBlue); break;
        //
        //            case "2/w": TwoOrWhite = Increment(TwoOrWhite); break;
        //            case "2/u": TwoOrBlue = Increment(TwoOrBlue); break;
        //            case "2/b": TwoOrBlack = Increment(TwoOrBlack); break;
        //            case "2/r": TwoOrRed = Increment(TwoOrRed); break;
        //            case "2/g": TwoOrGreen = Increment(TwoOrGreen); break;
        //
        //            default:
        //                if (int.TryParse(symbol, out int numericValue))
        //                    Colorless = Increment(Colorless, numericValue);
        //                else
        //                    throw new Exception($"Unrecognized mana symbol: {symbol}");
        //                break;
        //        }
        //    }
        //}
    }

    [DistilledValue] public int? Colorless { get; set; }
    [DistilledValue] public int? White { get; set; }
    [DistilledValue] public int? Blue { get; set; }
    [DistilledValue] public int? Black { get; set; }
    [DistilledValue] public int? Red { get; set; }
    [DistilledValue] public int? Green { get; set; }

    [DistilledValue] public int? HybridWhiteBlue { get; set; }     // {w/u}
    [DistilledValue] public int? HybridWhiteBlack { get; set; }    // {w/b}
    [DistilledValue] public int? HybridBlueBlack { get; set; }     // {u/b}
    [DistilledValue] public int? HybridBlueRed { get; set; }       // {u/r}
    [DistilledValue] public int? HybridBlackRed { get; set; }      // {b/r}
    [DistilledValue] public int? HybridBlackGreen { get; set; }    // {b/g}
    [DistilledValue] public int? HybridRedGreen { get; set; }      // {r/g}
    [DistilledValue] public int? HybridRedWhite { get; set; }      // {r/w}
    [DistilledValue] public int? HybridGreenWhite { get; set; }    // {g/w}
    [DistilledValue] public int? HybridGreenBlue { get; set; }     // {g/u}

    [DistilledValue] public int? TwoOrWhite { get; set; }          // {2/w}
    [DistilledValue] public int? TwoOrBlue { get; set; }           // {2/u}
    [DistilledValue] public int? TwoOrBlack { get; set; }          // {2/b}
    [DistilledValue] public int? TwoOrRed { get; set; }            // {2/r}
    [DistilledValue] public int? TwoOrGreen { get; set; }          // {2/g}

    [DistilledValue] public int? X { get; set; }                   // {x}
    [DistilledValue] public int? Phyrexian { get; set; }           // {p}
    [DistilledValue] public int? Snow { get; set; }                // {s}
    [DistilledValue] public int? Infinite { get; set; }            // {∞}
}
