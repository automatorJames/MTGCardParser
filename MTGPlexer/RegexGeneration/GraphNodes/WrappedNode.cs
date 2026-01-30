namespace MTGPlexer.RegexGeneration.GraphNodes;

public record WrappedNode : ValueNode
{
    public Type Type { get; }
    public object DifferentiatorValue { get; init; }
    public int Ordinal { get; init; }

    public WrappedNode(Node parentNode, Type type, int ordinal = 0, object differentiatorValue = null) : base(parentNode, type.Name, type)
    {
        Type = type;
        Ordinal = ordinal;
        DifferentiatorValue = differentiatorValue;
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        ConcatenatingComposer.Instance.Compose(builder, Children.ToList());
    }
}
