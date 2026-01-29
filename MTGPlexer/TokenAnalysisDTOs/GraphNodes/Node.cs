namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public abstract record Node
{
    public string Name { get; set; }
    public Node ParentNode { get; }

    public abstract void ComposeRegexLines(RegexBuilder collector);

    protected Node(Node parentNode, string name)
    {
        Name = name;
        ParentNode = parentNode;
    }
}