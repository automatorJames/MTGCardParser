namespace MTGPlexer.TokenUnits;

public class GainOrLoseBuffs : TokenUnit
{
    public GainOrLoseBuffs() : base("buh", nameof(GainedOrLostBuffs))
    {
    }

    public TokenUnitMany<GainedOrLostBuff> GainedOrLostBuffs { get; set; }
}

 