namespace MTGPlexer.TokenUnits;

public class CounterOnCard :TokenUnit
{
    public CounterOnCard() : base(nameof(CounterType), "counter") { }

    public CounterType CounterType { get; set; }
}