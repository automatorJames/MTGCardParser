namespace MTGCardParser.TokenCaptures;

public class ActivatedAbility : ITokenCapture
{
    public string RegexTemplate => $@"\(?(?<{nameof(ActivationCost)}>[^:]+): (?<{nameof(Effect)}>[^.]+)\.\)?";

    public TokenSegment ActivationCost { get; set; }
    public TokenSegment Effect { get; set; }
}