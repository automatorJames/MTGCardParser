namespace MTGCardParser.TokenCaptures;

public class ActivatedAbility : ITokenCapture
{
    public RegexTemplate<ActivatedAbility> RegexTemplate => new(nameof(ActivationCost), nameof(Effect));

    public ActivationCost ActivationCost { get; set; }

    [RegexPattern(@".+\.\)?")]
    public TokenSegment Effect { get; set; }
}