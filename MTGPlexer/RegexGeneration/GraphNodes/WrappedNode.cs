namespace MTGPlexer.RegexGeneration.GraphNodes;

public class WrappedNode : ValueNode
{
    public Type Type { get; }
    public object DifferentiatorValue { get; }
    public int Ordinal { get; }
    public int SiblingCaptureCount { get; }
    public CaptureNode WrappedCaptureNode { get; }

    public WrappedNode(Node parentNode, Type type, int ordinal = 0, int siblingCaptureCount = 0, object differentiatorValue = null) 
        : base(parentNode, GetUnderlyingType(type).Name)
    {
        Type = type;
        Ordinal = ordinal;
        SiblingCaptureCount = siblingCaptureCount;
        DifferentiatorValue = differentiatorValue;
        var name = GetUnderlyingType(type).Name + differentiatorValue?.ToString();
        VirtualNavigation navigation = new(name, type);

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

    public override CaptureValueInfo GetCaptureValueInfo(CaptureDictionary captureDictionary) =>
        WrappedCaptureNode.GetCaptureValueInfo(captureDictionary) with { Ordinal = Ordinal, SiblingCaptureCount = SiblingCaptureCount };
}
