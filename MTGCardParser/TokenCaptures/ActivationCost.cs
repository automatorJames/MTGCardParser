namespace MTGCardParser.TokenCaptures;

[NoSpaces]
public class ActivationCost : ITokenUnit
{
    public RegexTemplate<ActivationCost> RegexTemplate => new("^", nameof(ActivationCostSegment), ":");

    [RegexPattern("[^:]+")]
    public CapturedTextSegment ActivationCostSegment { get; set; }
}