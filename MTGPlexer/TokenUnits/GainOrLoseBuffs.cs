namespace MTGPlexer.TokenUnits;

public class GainOrLoseBuffs : TokenUnit
{
    public ManyOf<Buff> GainedOrLostBuffs { get; set; }
}