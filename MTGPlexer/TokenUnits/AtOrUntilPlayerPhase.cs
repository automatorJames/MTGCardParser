namespace MTGPlexer.TokenUnits;

[TokenizationOrder(0)]
public class AtOrUntilPlayerPhase : TokenUnit
{
    public AtOrUntilPlayerPhase() : base (nameof(ActivateOnly), nameof(TemporalDisposition), "the", nameof(PhasePart), "of", nameof(Whose), nameof(Phase)) { }

    [RegexPattern("activate only")]
    public bool ActivateOnly { get; set; }

    public TemporalDisposition TemporalDisposition { get; set; }
    public PhasePart PhasePart { get; set; }
    public Whose Whose { get; set; }
    public Phase Phase { get; set; }
}

public enum TemporalDisposition
{
    At,
    During,
    Until
}

public enum PhasePart
{
    Beginning,
    End
}

public enum Whose
{
    [RegexPattern("your opponent's")]
    YourOpponents,
    Your,
    Each
}

public enum Phase
{
    Upkeep,
    DrawStep,
    MainPhase,
    CombatPhase,
    CombatStep,
    DeclareAttackersStep,
    DeclareBlockersStep,
    DamageStep,
    EndStep,
    EndOfTurn
}