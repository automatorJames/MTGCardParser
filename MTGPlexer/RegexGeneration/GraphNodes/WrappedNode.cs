namespace MTGPlexer.RegexGeneration.GraphNodes;

public class WrappedNode : CaptureNode
{
    public Type Type { get; }
    public int? Ordinal { get; }
    public CaptureNode WrappedCaptureNode { get; }

    public override bool IsCollapsible => true;

    public WrappedNode(RegexNode parentNode, Type type, int? ordinal = null, string name = null) 
        : base(parentNode, new TypeNavigation(type, name))
    {
        Type = type;
        Ordinal = ordinal;
        TypeNavigation navigation = new(type);

        if (GetUnderlyingType(type).IsEnum)
            WrappedCaptureNode = new EnumNode(this, navigation);
        else if (type.IsAssignableTo(typeof(TokenUnit)))
            WrappedCaptureNode = new TokenUnitNode(this, navigation);
        else
            throw new Exception($"{nameof(WrappedNode)} can only be created from enum of {nameof(TokenUnit)} types, but '{type.Name}' was passed");
    }

    static Type GetUnderlyingType(Type type)
        => Nullable.GetUnderlyingType(type) ?? type;

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        WrappedCaptureNode.ComposeRegexLines(builder);
    }

    public override object GetValueAndSetHydrationInfo(CaptureContext captureContext)
    {
        var scopedCaptureContext = captureContext[FullyQualifiedName];
        var value = WrappedCaptureNode.GetValueAndSetHydrationInfo(scopedCaptureContext);
        CaptureValueHydrationInfo = new(this, scopedCaptureContext.Capture, value);

        return value;
    }
}
