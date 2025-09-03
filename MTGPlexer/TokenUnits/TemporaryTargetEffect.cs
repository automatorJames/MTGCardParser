namespace MTGPlexer.TokenUnits;

[Color("#ff00ff")]
public class TemporaryTargetEffect : TokenUnit
{
    public TemporaryTargetEffect() : base("target", nameof(CardType), nameof(PermanentVerb), nameof(Buff), "until", nameof(Phase)) { }

    public CardType CardType { get; set; }
    public PermanentVerb PermanentVerb { get; set; }
    public GainOrLoseBuffs Buff { get; set; }
    public Phase Phase { get; set; }

}

