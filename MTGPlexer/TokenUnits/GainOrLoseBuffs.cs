namespace MTGPlexer.TokenUnits;

public class GainOrLoseBuffs : TokenUnit
{
    public GainOrLoseBuffs() : base("buh", nameof(GainedOrLostBuffs))
    {
    }

    public ManyToken<Buff> GainedOrLostBuffs { get; set; }
}

 