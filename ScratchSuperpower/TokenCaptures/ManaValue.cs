namespace MTGCardParser.TokenCaptures;

public class ManaValue : ITokenCapture
{
    public string RegexTemplate => @"(?:\{(?<ManaSymbol>[0-9]+|[wubrgxyzc∞]|w/u|w/b|u/b|u/r|b/r|b/g|r/g|r/w|g/w|g/u|2/w|2/u|2/b|2/r|2/g|p|s)\})+";

    public ManaValue()
    {
    }

    public ManaValue(string tokenString)
    {
        PopulateScalarValues(tokenString);
    }

    public void PopulateScalarValues(string tokenString)
    {
        var matches = Regex.Matches(tokenString, RegexTemplate);

        foreach (Match match in matches)
        {
            var symbol = match.Groups["ManaSymbol"].Value.ToLowerInvariant();

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

    public int Colorless { get; set; }
    public int White { get; set; }
    public int Blue { get; set; }
    public int Black { get; set; }
    public int Red { get; set; }
    public int Green { get; set; }

    public int HybridWhiteBlue { get; set; }     // {w/u}
    public int HybridWhiteBlack { get; set; }    // {w/b}
    public int HybridBlueBlack { get; set; }     // {u/b}
    public int HybridBlueRed { get; set; }       // {u/r}
    public int HybridBlackRed { get; set; }      // {b/r}
    public int HybridBlackGreen { get; set; }    // {b/g}
    public int HybridRedGreen { get; set; }      // {r/g}
    public int HybridRedWhite { get; set; }      // {r/w}
    public int HybridGreenWhite { get; set; }    // {g/w}
    public int HybridGreenBlue { get; set; }     // {g/u}

    public int TwoOrWhite { get; set; }          // {2/w}
    public int TwoOrBlue { get; set; }           // {2/u}
    public int TwoOrBlack { get; set; }          // {2/b}
    public int TwoOrRed { get; set; }            // {2/r}
    public int TwoOrGreen { get; set; }          // {2/g}

    public int X { get; set; }                   // {x}
    public int Phyrexian { get; set; }           // {p}
    public int Snow { get; set; }                // {s}
    public int Infinite { get; set; }            // {∞}
}

