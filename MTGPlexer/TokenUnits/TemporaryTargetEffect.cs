namespace MTGPlexer.TokenUnits;

[Color("#ff00ff")]
public class TemporaryTargetEffect : TokenUnit
{
    protected override string[] Snippets => ["target", nameof(CardType), nameof(PermanentVerb), nameof(GainedOrLostBuffs), "until", nameof(Phase)];

    public CardType CardType { get; set; }
    public PermanentVerb PermanentVerb { get; set; }
    public ManyOf<Buff> GainedOrLostBuffs { get; set; }
    public Phase Phase { get; set; }
}