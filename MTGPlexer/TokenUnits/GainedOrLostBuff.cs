namespace MTGPlexer.TokenUnits;

[Dependent]
public class GainedOrLostBuff : TokenUnit
{
    public PermanentVerb PermanentVerb { get; set; }
    public Buff Buff { get; set; }
}