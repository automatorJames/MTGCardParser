namespace MTGPlexer.TokenUnits;

public class CounterOnCard : TokenUnit
{
    public override Snippet[] Snippets => [Prop(CounterType), "counter"];

    public CounterType CounterType { get; set; }
}