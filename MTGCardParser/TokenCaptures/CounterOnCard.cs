namespace MTGCardParser.TokenCaptures;

public class CounterOnCard : ITokenCapture
{
    public string RegexTemplate => $@"§{nameof(CounterType)}§ counter";

    public CounterType? CounterType { get; set; }
}