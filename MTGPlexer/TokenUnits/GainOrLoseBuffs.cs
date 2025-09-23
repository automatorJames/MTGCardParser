namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
public class GainOrLoseBuffs : TokenUnit
{
    public GainOrLoseBuffs() : base(nameof(GainedOrLostBuffs))
    {
    }

    public ManyOf<Buff> GainedOrLostBuffs { get; set; }
}

 