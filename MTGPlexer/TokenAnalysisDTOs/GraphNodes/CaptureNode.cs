namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public abstract record CaptureNode : Node
{
    public string Name { get; }
    public Node ParentNode { get; }
    public PropertySnippet PropertySnippet { get; }
    public Type UnderlyingType { get; }
    public Type[] GenericTypes { get; }
    public Proptions Proptions { get; }

    protected CaptureNode(Node parentNode, PropertySnippet propertySnippet)
    {
        Name = propertySnippet.Prop.Name;
        ParentNode = parentNode;
        PropertySnippet = propertySnippet;
        UnderlyingType = Nullable.GetUnderlyingType(propertySnippet.Prop.PropertyType) ?? propertySnippet.Prop.PropertyType;
        GenericTypes = UnderlyingType.GetGenericArguments();
    }

    public abstract void SetPropertyValue(MatchTraversalState parentTokenUnitMatch, ExtractedCapture scopedCapture, out ValueResult result);
}