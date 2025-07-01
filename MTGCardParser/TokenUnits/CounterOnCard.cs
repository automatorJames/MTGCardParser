using MTGCardParser.TokenUnits.Interfaces;

namespace MTGCardParser.TokenUnits;

public class CounterOnCard :ITokenUnit
{
    public RegexTemplate<CounterOnCard> RegexTemplate => new(nameof(CounterType), "counter");

    public CounterType? CounterType { get; set; }
}