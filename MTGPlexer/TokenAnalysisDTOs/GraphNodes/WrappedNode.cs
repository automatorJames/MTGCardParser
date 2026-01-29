namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public abstract record WrappedNode : Node
{
    public Type Type { get; }
    public object DifferentiatorValue { get; init; }

    public WrappedNode(Node parentNode, string name, Type type, object differentiatorValue = null) : base(parentNode, name)
    {
        Type = type;
        DifferentiatorValue = differentiatorValue;
    }
}
