namespace MTGCardParser.TokenUnits;

[NoSpaces]
public class ActivationCost : TokenUnit
{
    public RegexTemplate RegexTemplate => new("^", nameof(ActivationCostSegment), ":");

    [RegexPattern("[^:]+")]
    public PlaceholderCapture ActivationCostSegment { get; set; }
}