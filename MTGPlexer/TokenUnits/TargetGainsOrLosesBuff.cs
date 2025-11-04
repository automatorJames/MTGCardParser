namespace MTGPlexer.TokenUnits;

public class TargetGainsOrLosesBuff : TokenUnit
{
    protected override string[] Snippets => [nameof(TagetCardType), nameof(GainedOrLostBuff), "until end of turn"];

    public TagetCardType TagetCardType { get; set; }

    [OptionalMany]
    public GainedOrLostBuff GainedOrLostBuff { get; set; }
}