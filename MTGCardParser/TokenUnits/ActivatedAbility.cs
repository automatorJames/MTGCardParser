using MTGCardParser.TokenUnits.Interfaces;

namespace MTGCardParser.TokenUnits;

public class ActivatedAbility : ITokenUnit
{
    public RegexTemplate<ActivatedAbility> RegexTemplate => new(nameof(ActivationCost), nameof(Effect));

    public ActivationCost ActivationCost { get; set; }

    [RegexPattern(@".+\.\)?")]
    public CapturedTextSegment Effect { get; set; }
}