namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class CaptureNode : TypedNode
{
    public string FullyQualifiedName { get; }
    public INavigable Navigable { get; }
    public PropertyInfo ConcreteProperty { get; }
    public bool IsOptional { get; }
    public string[] OverrideRegexPatterns { get; }

    protected CaptureNode(Node parentNode, INavigable navigable)
        : base(parentNode, navigable.Name, navigable.Type)
    {
        FullyQualifiedName = GetFullyQualifiedCaptureGroupName();
        Navigable = navigable;
        ConcreteProperty = (navigable as PropertySnippet)?.Prop;

        if (navigable is PropertySnippet propertySnippet)
        {
            ConcreteProperty = propertySnippet.Prop;

            IsOptional =
                ConcreteProperty.IsDefined(typeof(OptionalComponentAttribute))
                || UnderlyingType.IsEnum && Nullable.GetUnderlyingType(ConcreteProperty.PropertyType) != null;

            OverrideRegexPatterns = ConcreteProperty.GetCustomAttribute<RegexPatternAttribute>()?.Patterns;
        }

        IsOptional |= navigable.Proptions.HasFlag(Proptions.Optional);
    }

    public void SetPropertyValue(CaptureDictionary captures, TokenUnit parent)
    {
        if (ConcreteProperty == null)
            throw new Exception($"{FullyQualifiedName} does not represent a concrete CLR property, so its value cannot be set");

        var capturesForName = captures[FullyQualifiedName];

        var value = GetValue(capturesForName);

        if (value == null)
            return;

        ConcreteProperty.SetValue(parent, value);
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
