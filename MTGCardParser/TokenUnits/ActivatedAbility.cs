namespace MTGCardParser.TokenUnits;

public class ActivatedAbility : TokenUnit
{
    public ActivationCost ActivationCost { get; set; }

    [RegexPattern(@".+\.\)?")]
    public PlaceholderCapture Effect { get; set; }

}