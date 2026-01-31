namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class WrapperPropertyNode : CaptureNode
{
    protected Type GenericType => GenericTypes[0];

    public WrapperPropertyNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }

    protected WrappedNode GetTemplateNodeForType(int ordinal = 0, int genericTypeIndex = 0, object differentiatorValue = null)
    {
        if (0 > GenericTypes.Length)
            throw new IndexOutOfRangeException();

        return new WrappedNode(this, GenericTypes[genericTypeIndex], ordinal, differentiatorValue);
    }
}