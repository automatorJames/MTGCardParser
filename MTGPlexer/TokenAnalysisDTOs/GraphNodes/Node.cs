namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public abstract record Node
{
    public abstract void ComposeRegexLines(RegexBuilder collector);

    protected Node()
    {
    }
}