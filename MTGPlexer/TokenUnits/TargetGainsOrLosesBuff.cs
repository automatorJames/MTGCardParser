namespace MTGPlexer.TokenUnits;

public class TargetGainsOrLosesBuff : TokenUnit
{
    public TagetCardType TagetCardType { get; set; }

    [OptionalMany]
    public GainedOrLostBuff GainedOrLostBuff { get; set; }
}