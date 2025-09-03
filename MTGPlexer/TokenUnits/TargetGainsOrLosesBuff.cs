namespace MTGPlexer.TokenUnits;

public class TargetGainsOrLosesBuff : TokenUnit
{
    public TargetGainsOrLosesBuff() : base(nameof(TagetCardType), nameof(GainedOrLostBuff), "until end of turn"){ }

    public TagetCardType TagetCardType { get; set; }

    [OptionalMany]
    public GainedOrLostBuff GainedOrLostBuff { get; set; }
}