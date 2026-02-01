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

        IsOptional = navigable.Proptions.HasFlag(Proptions.Optional)
            || UnderlyingType.IsEnum && Nullable.GetUnderlyingType(Navigable.Type) != null;

        if (navigable is PropertySnippet propertySnippet)
        {
            ConcreteProperty = propertySnippet.Prop;

            IsOptional |= ConcreteProperty.IsDefined(typeof(OptionalComponentAttribute)); 

            // Optional sub-groups aren't allowed in TokenUnitOneOf groups, because they would allow zero-width matches
            IsOptional &= !ConcreteProperty.DeclaringType.IsAssignableTo(typeof(TokenUnitOneOf));

            OverrideRegexPatterns = ConcreteProperty.GetCustomAttribute<RegexPatternAttribute>()?.Patterns;
        }
    }

    public void SetPropertyValue(CaptureDictionary captureDictionary, TokenUnit parent)
    {
        if (ConcreteProperty == null)
            throw new Exception($"{FullyQualifiedName} does not represent a concrete CLR property, so its value cannot be set");

        var value = GetCaptureValueInfo(captureDictionary);

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
