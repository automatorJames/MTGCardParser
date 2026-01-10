namespace MTGPlexer.TokenUnits;

public class TargetGainsOrLosesBuff : TokenUnit
{
    protected override Snippet[] Snippets => [Prop(TargetCard), Prop(GainedOrLostBuff), "until end of turn"];

    public TargetCard TargetCard { get; set; }

    [OptionalMany]
    public GainedOrLostBuff GainedOrLostBuff { get; set; }
}