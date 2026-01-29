namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public abstract record BranchNode : CaptureNode
{
    public List<Node> Children { get; set; }

    public BranchNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }
}