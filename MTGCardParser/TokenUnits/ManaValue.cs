namespace MTGCardParser.TokenUnits;

public class ManaValue : TokenUnit
{
    public RegexTemplate<ManaValue> RegexTemplate => new(nameof(ManaSymbols));

    [RegexPattern(@"(\{([0-9]+|[wubrgxyzc∞]|w/u|w/b|u/b|u/r|b/r|b/g|r/g|r/w|g/w|g/u|2/w|2/u|2/b|2/r|2/g|p|s)\})+")]
    //[RegexPattern(@"(?<ManaSymols>\{c\}\{c\}\{c\})")]
    public CapturedTextSegment ManaSymbols { get; set; }


    public override void SetPropertiesFromMatchSpan()
    {
        var matches = Regex.Matches(MatchSpan.ToStringValue(), RegexTemplate.RenderedRegexString);

        foreach (Match match in matches)
        {
            var symbols = match.Groups[nameof(ManaSymbols)].Value.ToLowerInvariant();
            var symbolsMatches = Regex.Matches(symbols, @"(?<=\{)[^{}]+(?=\})");

            foreach (var symbol in symbolsMatches.Select(x => x.Value))
            {

                switch (symbol)
                {
                    case "w": White++; break;
                    case "u": Blue++; break;
                    case "b": Black++; break;
                    case "r": Red++; break;
                    case "g": Green++; break;
                    case "x": X++; break;
                    case "c": Colorless++; break;
                    case "∞": Infinite++; break;
                    case "p": Phyrexian++; break;
                    case "s": Snow++; break;

                    case "w/u": HybridWhiteBlue++; break;
                    case "w/b": HybridWhiteBlack++; break;
                    case "u/b": HybridBlueBlack++; break;
                    case "u/r": HybridBlueRed++; break;
                    case "b/r": HybridBlackRed++; break;
                    case "b/g": HybridBlackGreen++; break;
                    case "r/g": HybridRedGreen++; break;
                    case "r/w": HybridRedWhite++; break;
                    case "g/w": HybridGreenWhite++; break;
                    case "g/u": HybridGreenBlue++; break;

                    case "2/w": TwoOrWhite++; break;
                    case "2/u": TwoOrBlue++; break;
                    case "2/b": TwoOrBlack++; break;
                    case "2/r": TwoOrRed++; break;
                    case "2/g": TwoOrGreen++; break;

                    default:
                        if (int.TryParse(symbol, out int numericValue))
                            Colorless += numericValue;
                        else
                            throw new Exception($"Unrecognized mana symbol: {symbol}");
                        break;
                }
            }
        }
    }

    [DistilledValue] public int Colorless { get; set; }
    [DistilledValue] public int White { get; set; }
    [DistilledValue] public int Blue { get; set; }
    [DistilledValue] public int Black { get; set; }
    [DistilledValue] public int Red { get; set; }
    [DistilledValue] public int Green { get; set; }
    
    [DistilledValue] public int HybridWhiteBlue { get; set; }     // {w/u}
    [DistilledValue] public int HybridWhiteBlack { get; set; }    // {w/b}
    [DistilledValue] public int HybridBlueBlack { get; set; }     // {u/b}
    [DistilledValue] public int HybridBlueRed { get; set; }       // {u/r}
    [DistilledValue] public int HybridBlackRed { get; set; }      // {b/r}
    [DistilledValue] public int HybridBlackGreen { get; set; }    // {b/g}
    [DistilledValue] public int HybridRedGreen { get; set; }      // {r/g}
    [DistilledValue] public int HybridRedWhite { get; set; }      // {r/w}
    [DistilledValue] public int HybridGreenWhite { get; set; }    // {g/w}
    [DistilledValue] public int HybridGreenBlue { get; set; }     // {g/u}
    
    [DistilledValue] public int TwoOrWhite { get; set; }          // {2/w}
    [DistilledValue] public int TwoOrBlue { get; set; }           // {2/u}
    [DistilledValue] public int TwoOrBlack { get; set; }          // {2/b}
    [DistilledValue] public int TwoOrRed { get; set; }            // {2/r}
    [DistilledValue] public int TwoOrGreen { get; set; }          // {2/g}
    
    [DistilledValue] public int X { get; set; }                   // {x}
    [DistilledValue] public int Phyrexian { get; set; }           // {p}
    [DistilledValue] public int Snow { get; set; }                // {s}
    [DistilledValue] public int Infinite { get; set; }            // {∞}
}

