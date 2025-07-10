namespace MTGCardParser.TokenUnits;

public class CounterOnCard :TokenUnit
{
    public RegexTemplate RegexTemplate => new(nameof(CounterType), "counter");

    public CounterType CounterType { get; set; }
}