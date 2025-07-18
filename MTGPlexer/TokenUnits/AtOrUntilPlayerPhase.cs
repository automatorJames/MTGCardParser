namespace MTGPlexer.TokenUnits;

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
    [RegexPattern("upkeep")]
    Upkeep,

    [RegexPattern("draw step")]
    DrawStep,

    [RegexPattern("main phase")]
    MainPhase,

    [RegexPattern("combat phase")]
    CombatPhase,

    [RegexPattern("combat step")]
    CombatStep,

    [RegexPattern("declare attackers step")]
    DeclareAttackersStep,

    [RegexPattern("declare blockers step")]
    DeclareBlockersStep,

    [RegexPattern("damage step")]
    DamageStep,

    [RegexPattern("end step")]
    EndStep,

    [RegexPattern("end of turn")]
    EndOfTurn
}