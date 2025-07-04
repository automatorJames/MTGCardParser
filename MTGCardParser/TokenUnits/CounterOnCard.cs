namespace MTGCardParser.TokenUnits;

public class CounterOnCard :TokenUnitBase
{
    public RegexTemplate<CounterOnCard> RegexTemplate => new(nameof(CounterType), "counter");

    public CounterType? CounterType { get; set; }
}