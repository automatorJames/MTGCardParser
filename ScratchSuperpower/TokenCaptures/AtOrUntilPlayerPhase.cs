namespace MTGCardParser.TokenCaptures;

public class AtOrUntilPlayerPhase : ITokenCapture
{
    public string RegexTemplate => $@"§{nameof(ActivateOnly)}§§{nameof(TemporalDisposition)}§ the §{nameof(PhasePart)}§ of §{nameof(Whose)}§ §{nameof(Phase)}§";

    public TemporalDisposition? TemporalDisposition { get; set; }
    public PhasePart? PhasePart { get; set; }
    public Whose? Whose { get; set; }
    public Phase? Phase { get; set; }

    [RegPat("activate only")]
    public bool ActivateOnly { get; set; }
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
    [RegPat("your opponent's")]
    YourOpponents,
    Your,
    Each
}

public enum Phase
{
    [RegPat("upkeep")]
    Upkeep,

    [RegPat("draw step")]
    DrawStep,

    [RegPat("main phase")]
    MainPhase,

    [RegPat("combat phase")]
    CombatPhase,

    [RegPat("combat step")]
    CombatStep,

    [RegPat("declare attackers step")]
    DeclareAttackersStep,

    [RegPat("declare blockers step")]
    DeclareBlockersStep,

    [RegPat("damage step")]
    DamageStep,

    [RegPat("end step")]
    EndStep,

    [RegPat("end of turn")]
    EndOfTurn
}