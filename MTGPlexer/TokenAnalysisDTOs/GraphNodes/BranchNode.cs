namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public abstract record BranchNode : ParentNode
{
    public string Name { get; }
    public PropertySnippet PropertySnippet { get; }
    public Type UnderlyingType { get; }
    public Type[] GenericTypes { get; }
    public Proptions Proptions { get; } = Proptions.None;

    public BranchNode(PropertySnippet propertySnippet)
    {
        Name = propertySnippet.Prop.Name;
        PropertySnippet = propertySnippet;
        var underlyingType = Nullable.GetUnderlyingType(PropertySnippet.Prop.PropertyType) ?? PropertySnippet.Prop.PropertyType;
        UnderlyingType = underlyingType;
        GenericTypes = underlyingType.GetGenericArguments();
    }
}