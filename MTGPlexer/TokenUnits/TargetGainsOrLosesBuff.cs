namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
public class TargetGainsOrLosesBuff : TokenUnit
{
    public override Snippet[] Snippets => [Prop(TargetCard), Prop(GainedOrLostBuff), "until end of turn"];

    public TargetCard TargetCard { get; set; }
    public ManyOf<GainedOrLostBuff> GainedOrLostBuff { get; set; }
}