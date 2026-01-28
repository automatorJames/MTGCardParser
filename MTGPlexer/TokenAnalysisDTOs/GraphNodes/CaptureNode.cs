namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public abstract record CaptureNode : Node
{
    public string Name { get; }
    public PropertySnippet PropertySnippet { get; }
    public Type UnderlyingType { get; }
    public Type[] GenericTypes { get; }
    public Proptions Proptions { get; }

    protected CaptureNode(PropertySnippet propertySnippet)
    {
        Name = propertySnippet.Prop.Name;
        PropertySnippet = propertySnippet;
        UnderlyingType = Nullable.GetUnderlyingType(propertySnippet.Prop.PropertyType) ?? propertySnippet.Prop.PropertyType;
        GenericTypes = UnderlyingType.GetGenericArguments();
    }

    public abstract object GetPropertyValue(MatchTraversalState parentTokenUnitMatch, ExtractedCapture scopedCapture, out ValueResult result);
}