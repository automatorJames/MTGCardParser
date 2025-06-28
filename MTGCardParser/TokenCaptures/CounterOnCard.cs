namespace MTGCardParser.TokenCaptures;

public class CounterOnCard : TokenCaptureBase<CounterOnCard>
{
    public override RegexTemplate<CounterOnCard> RegexTemplate => new(nameof(CounterType), "counter");

    public CounterType? CounterType { get; set; }
}