namespace MTGCardParser.TokenCaptures;

public class ActivatedAbility : ITokenCapture
{
    public static string RegexTemplate => $@"(?<{nameof(ActivationCost)}>[^:]+):\s*(?<{nameof(Effect)}>[^.]+)\.";

    public TokenSegment ActivationCost { get; set; }
    public TokenSegment Effect { get; set; }
}