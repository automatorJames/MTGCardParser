namespace MTGPlexer.TokenUnits;

public class CounterOnCard : TokenUnit
{
    protected override Snippet[] Snippets => [Prop(CounterType), "counter"];

    public CounterType CounterType { get; set; }
}