namespace MTGPlexer.TokenUnits;

[NoSpaces]
public class ActivationCost : TokenUnit
{
    protected override string[] Snippets => ["^", nameof(ActivationCostSegment), ":"];

    [RegexPattern("[^:]+")]
    public PlaceholderCapture ActivationCostSegment { get; set; }
}