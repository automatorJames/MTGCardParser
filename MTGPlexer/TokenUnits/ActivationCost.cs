namespace MTGPlexer.TokenUnits;

[NoSpaces]
public class ActivationCost : TokenUnit
{
    public ActivationCost() : base("^", nameof(ActivationCostSegment), ":") { }

    [RegexPattern("[^:]+")]
    public PlaceholderCapture ActivationCostSegment { get; set; }
}