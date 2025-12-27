namespace MTGPlexer.TokenUnits;

public class TargetGainsOrLosesBuff : TokenUnit
{
    protected override string[] Snippets => [nameof(TagetCardType), nameof(GainedOrLostBuff), "until end of turn"];

    public TagetCard TagetCardType { get; set; }

    [OptionalMany]
    public GainedOrLostBuff GainedOrLostBuff { get; set; }
}