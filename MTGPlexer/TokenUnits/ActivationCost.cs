namespace MTGPlexer.TokenUnits;

[NoSpaces]
public class ActivationCost : TokenUnit
{
    protected override Snippet[] Snippets => ["^", Prop(ActivationCostSegment), ":"];

    [RegexPattern("[^:]+")]
    public PlaceholderCapture ActivationCostSegment { get; set; }
}