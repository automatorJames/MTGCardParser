namespace MTGCardParser.TokenUnits;

public class CounterOnCard :TokenUnit
{
    public RegexTemplate<CounterOnCard> RegexTemplate => new(nameof(CounterType), "counter");

    public CounterType? CounterType { get; set; }
}