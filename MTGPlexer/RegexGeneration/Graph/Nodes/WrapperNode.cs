namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class WrapperNode : NamedGroupNode
{
    Type _closedWrapperType;
    protected IEnumerable<object> WrappedValues => 
        WrappedNodes.Select(x => x.CaptureValueHydrationInfo?.Value).Where(x => x != null);

    public List<WrappedNode> WrappedNodes { get; } = [];
    protected Type GenericType => GenericTypes[0];

    public WrapperNode(RegexNode parentNode, TypeNavigation navigation) 
        : base(parentNode, navigation)
    {
        _closedWrapperType = navigable.Type.GetGenericTypeDefinition().MakeGenericType(GenericTypes);
    }

    protected WrappedNode GetTemplateNodeForType(int genericTypeIndex = 0) =>
        new WrappedNode(this, GenericTypes[genericTypeIndex]);

    public WrappedNode AddNewWrappedNode(CaptureContext captureContext, int? ordinal = null, Type genericType = null)
    {
        genericType ??= GenericType;
        WrappedNode wrappedNode = new(this, genericType, ordinal);
        wrappedNode.GetValueAndSetHydrationInfo(captureContext);
        WrappedNodes.Add(wrappedNode);
        return wrappedNode;
    }

    protected object CreateWrapperValue() => CreateWrapperValue(WrappedValues);

    protected object CreateWrapperValue(params object[] constructorParameters) =>
        Activator.CreateInstance(_closedWrapperType, constructorParameters);

    public override object GetValueAndSetHydrationInfo(CaptureContext captureContext)
    {
        var scopedCaptureContext = captureContext[FullyQualifiedName];
        var value = GetWrapperValue(scopedCaptureContext);

        if (value != null)
            CaptureValueHydrationInfo = new(this, scopedCaptureContext.Capture, value);

        return value;
    }

    protected abstract object GetWrapperValue(CaptureContext captureContext);
}