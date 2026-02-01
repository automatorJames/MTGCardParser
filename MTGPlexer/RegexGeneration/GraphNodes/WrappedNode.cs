namespace MTGPlexer.RegexGeneration.GraphNodes;

public class WrappedNode : CaptureNode
{
    public Type Type { get; }
    public object DifferentiatorValue { get; }
    public int? Ordinal { get; }
    public CaptureNode WrappedCaptureNode { get; }

    public WrappedNode(Node parentNode, Type type, int? ordinal = null, object differentiatorValue = null) 
        : base(parentNode, new TypeNavigation(type))
    {
        Type = type;
        Ordinal = ordinal;
        DifferentiatorValue = differentiatorValue;
        var name = GetUnderlyingType(type).Name + differentiatorValue?.ToString();
        TypeNavigation navigation = new(type);

        if (GetUnderlyingType(type).IsEnum)
            WrappedCaptureNode = new EnumNode(this, navigation);
        else if (type.IsAssignableTo(typeof(TokenUnit)))
            WrappedCaptureNode = new TokenUnitNode(this, navigation);
        else
            throw new Exception($"{nameof(WrappedNode)} can only be created from enum of {nameof(TokenUnit)} types, but '{type.Name}' was passed");
    }

    public void HydrateFromCapture(Capture capture)
    {
        var value = WrappedCaptureNode.GetValueSingleCapture(capture);
        CaptureValueHydrationInfo = new(WrappedCaptureNode, capture, value);
    }

    static Type GetUnderlyingType(Type type)
        => Nullable.GetUnderlyingType(type) ?? type;

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        WrappedCaptureNode.ComposeRegexLines(builder);
    }

    public override object TryGetValue(CaptureDictionary captureDictionary, out CaptureValueResult result) =>
        WrappedCaptureNode.TryGetValue(captureDictionary, out result);

    public override object GetValueSingleCapture(Capture capture)
        => WrappedCaptureNode.CaptureValueHydrationInfo?.Value ?? WrappedCaptureNode.GetValueSingleCapture(capture);
}
