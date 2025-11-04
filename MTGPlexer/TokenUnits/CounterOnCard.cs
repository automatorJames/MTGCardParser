namespace MTGPlexer.TokenUnits;

public class CounterOnCard : TokenUnit
{
    protected override string[] Snippets => [nameof(CounterType), "counter"];

    public CounterType CounterType { get; set; }
}