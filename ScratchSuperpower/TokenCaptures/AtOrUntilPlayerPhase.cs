namespace MTGCardParser.TokenCaptures;

public class AtOrUntilPlayerPhase : ITokenCapture
{
    public string RegexTemplate => $@"§{nameof(AtOrUntil)}§ the §{nameof(When)}§ of §{nameof(Whose)}§ §{nameof(Phase)}§";

    public AtOrUntil? AtOrUntil { get; set; }
    public When? When { get; set; }
    public Whose? Whose { get; set; }
    public Phase? Phase { get; set; }
}

public enum AtOrUntil
{
    At,
    Until
}

public enum When
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