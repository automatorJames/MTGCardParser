namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public abstract record WrapperPropertyNode : CaptureNode
{
    protected List<WrappedNode> TemplateNodesForComposition = [];
    protected WrappedNode TemplateNodeForComposition => TemplateNodesForComposition.Single();
    protected Type GenericType => GenericTypes[0];

    public WrapperPropertyNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
        foreach (var genericType in GenericTypes)
            TemplateNodesForComposition.Add(new WrappedNode(this, GenericType));
    }
}