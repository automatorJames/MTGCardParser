namespace MTGCardParser.TokenCaptures;

public class YouMayPay : ITokenCapture
{
    public RegexTemplate<YouMayPay> RegexTemplate => new("you may pay", nameof(ManaValue), ".");

    public ManaValue ManaValue { get; set; }
}

