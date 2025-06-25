namespace MTGCardParser.TokenCaptures;

public class LoseOrGainAbility : ITokenCapture
{
    public string RegexTemplate => $@"§{nameof(LoseOrGain)}§ ""(?<{nameof(Ability)}>[^""]+)""";

    public TokenSegment Ability { get; set; }
    public LoseOrGain? LoseOrGain { get; set; }
}

public enum LoseOrGain
{
    [RegPat("loses?")]
    Lose,

    [RegPat("gains?")]
    Gain
}