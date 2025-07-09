namespace MTGCardParser.TokenUnits;

public class ActivatedAbility : TokenUnit
{
    public RegexTemplate<ActivatedAbility> RegexTemplate => new(nameof(ActivationCost), nameof(Effect));

    public ActivationCost ActivationCost { get; set; }

    [RegexPattern(@".+\.\)?")]
    public PlaceholderCapture Effect { get; set; }

}