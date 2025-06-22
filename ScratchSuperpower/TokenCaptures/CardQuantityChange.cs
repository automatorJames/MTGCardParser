namespace MTGCardParser.TokenCaptures;

public class CardQuantityChange : ITokenCapture
{
    public string RegexTemplate => $@"(?<verb>discard|discards|draw|draws)\s+(?<amount>a|\d+|one|two|three|four|five|six|seven|eight|nine|ten)\s+card(s)?";

    public Keyword? Keyword { get; set; }
}