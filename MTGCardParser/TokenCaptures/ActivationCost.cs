namespace MTGCardParser.TokenCaptures;

[NoSpaces]
public class ActivationCost : ITokenCapture
{
    public RegexTemplate<ActivationCost> RegexTemplate => new("^", nameof(ActivationCostSegment), ":");

    [RegexPattern("[^:]+")]
    public TokenSegment ActivationCostSegment { get; set; }
}