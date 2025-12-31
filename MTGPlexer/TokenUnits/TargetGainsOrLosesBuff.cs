namespace MTGPlexer.TokenUnits;

public class TargetGainsOrLosesBuff : TokenUnit
{
    protected override string[] Snippets => [nameof(TargetCardType), nameof(GainedOrLostBuff), "until end of turn"];

    public TargetCard TargetCardType { get; set; }

    [OptionalMany]
    public GainedOrLostBuff GainedOrLostBuff { get; set; }
}