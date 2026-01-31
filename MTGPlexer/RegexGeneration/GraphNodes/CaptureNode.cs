namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class CaptureNode : ValueNode
{
    public string FullyQualifiedName { get; }
    public PropertySnippet PropertySnippet { get; }
    public Proptions Proptions { get; }

    protected CaptureNode(Node parentNode, PropertySnippet propertySnippet)
        : base(parentNode, propertySnippet.Name, propertySnippet.Prop.PropertyType)
    {
        FullyQualifiedName = GetFullyQualifiedCaptureGroupName();
        PropertySnippet = propertySnippet;
    }

    public void SetPropertyValue(CaptureDictionary captures, TokenUnit parent)
    {
        var capturesForName = captures[FullyQualifiedName];

        var value = GetValue(capturesForName);

        if (value == null)
            return;

        PropertySnippet.Prop.SetValue(parent, value);
    }

    string GetFullyQualifiedCaptureGroupName()
    {
        List<string> parts = [];
        Node current = this;

        while (current != null)
        {
            parts.Add(current.Name);
            current = current.ParentNode;
        }

        parts.Reverse();
        parts = parts.Skip(1).ToList();
        return string.Join('_', parts);
    }
}