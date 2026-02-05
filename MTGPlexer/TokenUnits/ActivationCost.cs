namespace MTGPlexer.TokenUnits;

[NoSpaces]
public class ActivationCost : TokenUnit
{
    protected override Snippet[] Snippets => ["^", Prop(ActivationCostSegment), ":"];

    [RegexPattern("[^:]+")]
    public PrecursorCapture ActivationCostSegment { get; set; }
}