namespace MTGCardParser.TokenUnits;

[NoSpaces]
public class ActivationCost : TokenUnitBase
{
    public RegexTemplate<ActivationCost> RegexTemplate => new("^", nameof(ActivationCostSegment), ":");

    [RegexPattern("[^:]+")]
    public CapturedTextSegment ActivationCostSegment { get; set; }
}