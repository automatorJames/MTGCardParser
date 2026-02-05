namespace MTGPlexer.TokenUnits;

public class ActivatedAbility : TokenUnit
{
    public ActivationCost ActivationCost { get; set; }

    [RegexPattern(@".+\.\)?")]
    public PrecursorCapture Effect { get; set; }

}