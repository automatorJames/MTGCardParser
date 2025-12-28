namespace MTGPlexer.TokenUnits;

[TokenizationOrder(0)]
public class AtOrUntilPlayerPhase : TokenUnit
{
    protected override string[] Snippets => [nameof(TemporalDisposition), "the", nameof(PhasePart), "of", nameof(Whose), nameof(Phase)];

    public TemporalDisposition TemporalDisposition { get; set; }
    public PhasePart PhasePart { get; set; }
    public Whose Whose { get; set; }
    public Phase Phase { get; set; }
}