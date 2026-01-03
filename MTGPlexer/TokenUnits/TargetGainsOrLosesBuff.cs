namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
public class TargetGainsOrLosesBuff : TokenUnit
{
    protected override string[] Snippets => [nameof(TargetCard), nameof(GainedOrLostBuff), "until end of turn"];

    public TargetCard TargetCard { get; set; }

    [OptionalMany]
    public GainedOrLostBuff GainedOrLostBuff { get; set; }
}