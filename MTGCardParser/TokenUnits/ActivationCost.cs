using MTGCardParser.TokenUnits.Interfaces;

namespace MTGCardParser.TokenUnits;

[NoSpaces]
public class ActivationCost : ITokenUnit
{
    public RegexTemplate<ActivationCost> RegexTemplate => new("^", nameof(ActivationCostSegment), ":");

    [RegexPattern("[^:]+")]
    public CapturedTextSegment ActivationCostSegment { get; set; }
}