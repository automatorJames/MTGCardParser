namespace MTGPlexer.RegexGeneration.GraphNodes;

public class WrappedNode : ValueNode
{
    public Type Type { get; }
    public object DifferentiatorValue { get; }
    public int Ordinal { get; }
    public CaptureNode WrappedCaptureNode { get; }

    public WrappedNode(Node parentNode, Type type, int ordinal = 0, object differentiatorValue = null) : base(parentNode, type.Name)
    {
        Type = type;
        Ordinal = ordinal;
        DifferentiatorValue = differentiatorValue;

        var name = type.Name + differentiatorValue?.ToString();
        VirtualNavigation navigation = new(name, type);

        if (type.IsEnum)
            WrappedCaptureNode = new EnumNode(this, navigation);
        else if (type.IsAssignableTo(typeof(TokenUnit)))
            WrappedCaptureNode = new TokenUnitNode(this, navigation);
        else
            throw new Exception($"{nameof(WrappedNode)} can only be created from enum of {nameof(TokenUnit)} types, but '{type.Name}' was passed");
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        WrappedCaptureNode.ComposeRegexLines(builder);
    }
}
