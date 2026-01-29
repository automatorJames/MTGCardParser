namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public abstract record CaptureNode : TypedNode
{
    public PropertySnippet PropertySnippet { get; }
    public Proptions Proptions { get; }

    protected CaptureNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet.Prop.Name, propertySnippet.Prop.PropertyType)
    {
        PropertySnippet = propertySnippet;
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