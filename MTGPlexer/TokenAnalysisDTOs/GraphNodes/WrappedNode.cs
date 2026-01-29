
namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public record WrappedNode : TypedNode
{
    public Type Type { get; }
    public object DifferentiatorValue { get; init; }

    public WrappedNode(Node parentNode, Type type, object differentiatorValue = null) : base(parentNode, type.Name, type)
    {
        Type = type;
        DifferentiatorValue = differentiatorValue;
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        ConcatenatingComposer.Instance.Compose(builder, Children.ToList());
    }
}
