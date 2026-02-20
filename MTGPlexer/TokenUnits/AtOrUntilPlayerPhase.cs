namespace MTGPlexer.TokenUnits;

[TokenizationOrder(0)]
public class AtOrUntilPlayerPhase : TokenUnit
{
    public override Snippet[] Snippets => [Prop(TemporalDisposition), "the", Prop(PhasePart), "of", Prop(Whose), Prop(Phase)];

    public TemporalDisposition TemporalDisposition { get; set; }
    public PhasePart PhasePart { get; set; }
    public Whose Whose { get; set; }
    public Phase Phase { get; set; }
}