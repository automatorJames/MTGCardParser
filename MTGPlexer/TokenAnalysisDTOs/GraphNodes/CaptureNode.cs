namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public abstract record CaptureNode : Node
{
    public PropertySnippet PropertySnippet { get; }
    public Type UnderlyingType { get; }
    public Proptions Proptions { get; }

    protected CaptureNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet.Prop.Name)
    {
        PropertySnippet = propertySnippet;
        UnderlyingType = Nullable.GetUnderlyingType(propertySnippet.Prop.PropertyType) ?? propertySnippet.Prop.PropertyType;
    }

    public void SetPropertyValue(Match match, TokenUnit parent)
    {
        var fullyQualifiedName = GetFullyQualifiedName();
        var capture = match.Groups[fullyQualifiedName];
        var value = GetPropertyValue(capture);

        if (value == null)
            return;

        PropertySnippet.Prop.SetValue(parent, value);
    }

    string GetFullyQualifiedName()
    {
        List<string> parts = [];
        var current = ParentNode;

        while (current != null)
        {
            parts.Add(current.Name);
            current = current.ParentNode;
        }

        parts.Reverse();
        return string.Join('.', parts);
    }

    protected abstract object GetPropertyValue(Capture capture);
}