namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class WrapperNode : CaptureNode
{
    Type _closedWrapperType;
    protected IEnumerable<object> WrappedValues => 
        WrappedNodes.Select(x => x.CaptureValueHydrationInfo?.Value).Where(x => x != null);

    public List<WrappedNode> WrappedNodes { get; } = [];
    protected Type GenericType => GenericTypes[0];

    public WrapperNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
        _closedWrapperType = propertySnippet.Prop.PropertyType.GetGenericTypeDefinition().MakeGenericType(GenericTypes);
    }

    protected WrappedNode GetTemplateNodeForType(int genericTypeIndex = 0, object differentiatorValue = null)
    {
        if (0 > GenericTypes.Length)
            throw new IndexOutOfRangeException();

        return new WrappedNode(this, GenericTypes[genericTypeIndex], differentiatorValue: differentiatorValue);
    }

    public WrappedNode AddNewWrappedNode(Capture capture, int? ordinal = null, object differentiatorValue = null, Type genericType = null)
    {
        genericType ??= GenericType;
        WrappedNode wrappedNode = new(this, genericType, ordinal, differentiatorValue);
        wrappedNode.HydrateFromCapture(capture);
        WrappedNodes.Add(wrappedNode);
        return wrappedNode;
    }

    protected object CreateWrapperValue() => CreateWrapperValue(WrappedValues);

    protected object CreateWrapperValue(params object[] constructorParameters) =>
        Activator.CreateInstance(_closedWrapperType, constructorParameters);
}