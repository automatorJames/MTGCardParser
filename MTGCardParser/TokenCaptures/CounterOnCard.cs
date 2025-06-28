namespace MTGCardParser.TokenCaptures;

public class CounterOnCard :ITokenCapture
{
    public RegexTemplate<CounterOnCard> RegexTemplate => new(nameof(CounterType), "counter");

    public CounterType? CounterType { get; set; }
}