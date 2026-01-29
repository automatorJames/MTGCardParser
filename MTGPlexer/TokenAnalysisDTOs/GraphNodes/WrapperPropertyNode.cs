namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public abstract record WrapperPropertyNode : CaptureNode
{
    protected List<WrappedNode> TemplateNodesForComposition = [];
    protected WrappedNode TemplateNodeForComposition => TemplateNodesForComposition.Single();
    protected Type[] GenericTypes { get; }
    protected Type GenericType => GenericTypes[0];

    public WrapperPropertyNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
        GenericTypes = UnderlyingType.GetGenericArguments();

        foreach (var genericType in GenericTypes)
        {
            if (GenericType.IsAssignableTo(typeof(TokenUnit)))
                TemplateNodesForComposition.Add(new WrappedTokenUnitNode(this, GenericType));
            else if (GenericType.IsEnum)
                TemplateNodesForComposition.Add(new WrappedEnumNode(this, GenericType));
            else
                throw new Exception($"{nameof(WrapperPropertyNode)} may only be derived from TokenUnit or be an enum");
        }
    }
}