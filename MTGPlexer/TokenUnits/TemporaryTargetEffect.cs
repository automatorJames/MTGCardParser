namespace MTGPlexer.TokenUnits;

[Color("#ff00ff")]
public class TemporaryTargetEffect : TokenUnit
{
    public override Snippet[] Snippets => ["target", Prop(CardType), Prop(PermanentVerb), Prop(GainedOrLostBuffs), "until", Prop(Phase)];

    public CardType CardType { get; set; }
    public PermanentVerb PermanentVerb { get; set; }
    public ManyOf<Buff> GainedOrLostBuffs { get; set; }
    public Phase Phase { get; set; }
}