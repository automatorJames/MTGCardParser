namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class WrappedNode : BranchNode
{
    public Type Type { get; }
    public int? Ordinal { get; }
    public NamedGroupNode UnderlyingNode { get; }

    public override bool IsCollapsible => true;

    public WrappedNode(RegexNode parentNode, Type type, int? ordinal = null, string name = null) 
        : base(parentNode, new TypeNavigation(type, name))
    {
        Type = type;
        Ordinal = ordinal;
        TypeNavigation navigation = new(type);

        if (GetUnderlyingType(type).IsEnum)
            UnderlyingNode = new EnumNode(this, navigation);
        else if (type.IsAssignableTo(typeof(TokenUnit)))
            UnderlyingNode = new TokenUnitNode(this, navigation);
        else
            throw new Exception($"{nameof(Nodes.WrappedNode)} can only be created from enum of {nameof(TokenUnit)} types, but '{type.Name}' was passed");
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        WrappedNode.ComposeRegexLines(builder);
        throw new NotImplementedException();
    }

    public override object GetValueAndSetHydrationInfo(CaptureContext captureContext)
    {
        var scopedCaptureContext = captureContext[FullyQualifiedName];
        var value = WrappedNode.GetValueAndSetHydrationInfo(scopedCaptureContext);
        CaptureValueHydrationInfo = new(this, scopedCaptureContext.Capture, value);

        return value;
    }
}
