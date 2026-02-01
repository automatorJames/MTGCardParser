namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class WrapperPropertyNode : CaptureNode
{
    public List<WrappedNode> WrappedNodes { get; } = [];
    public override List<Node> Children => WrappedNodes.Cast<Node>().ToList();

    protected Type GenericType => GenericTypes[0];

    public WrapperPropertyNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }

    protected WrappedNode GetTemplateNodeForType(int genericTypeIndex = 0, object differentiatorValue = null)
    {
        if (0 > GenericTypes.Length)
            throw new IndexOutOfRangeException();

        return new WrappedNode(this, GenericTypes[genericTypeIndex], differentiatorValue: differentiatorValue);
    }

    protected CaptureValueInfo GetWrappedValue(
        CaptureDictionary captureDictionary,
        int genericTypeIndex = 0, 
        int ordinal = 0,
        int siblingCaptureCount = 1,
        object differentiatorValue = null
        )
    {
        WrappedNode wrappedNode = new(this, GenericTypes[genericTypeIndex], ordinal, siblingCaptureCount, differentiatorValue);
        return wrappedNode.GetCaptureValueInfo(captureDictionary);
    }
}