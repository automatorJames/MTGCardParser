namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public abstract record BranchNode : Node
{
    public string Name { get; }
    public Type UnderlyingType { get; }
    public Type[] GenericTypes { get; }
    public Proptions Proptions { get; } = Proptions.None;
    public List<Node> Children { get; } = [];

    public BranchNode(Type type)
    {
        Name = type.Name;
        UnderlyingType = type;
        GenericTypes = type.GetGenericArguments();
    }

    public BranchNode(PropertyInfo prop) : base(prop)
    {
        var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        UnderlyingType = underlyingType;
        GenericTypes = underlyingType.GetGenericArguments();
        Name = prop.Name;
    }
}