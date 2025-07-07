namespace MTGCardParser.TokenUnits;

[NoSpaces]
public class ActivationCost : TokenUnit
{
    public RegexTemplate<ActivationCost> RegexTemplate => new("^", nameof(ActivationCostSegment), ":");

    [RegexPattern("[^:]+")]
    public CapturedTextSegment ActivationCostSegment { get; set; }
}