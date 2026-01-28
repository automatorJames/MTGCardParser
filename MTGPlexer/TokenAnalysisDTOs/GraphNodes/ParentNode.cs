namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public abstract record ParentNode : Node
{
    public List<Node> Children { get; set; }
    public IEnumerable<CaptureNode> CaptureChildren => Children.OfType<CaptureNode>();
}
