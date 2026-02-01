namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class CaptureNode : Node
{
    public CaptureValueHydrationInfo CaptureValueHydrationInfo { get; protected set; }
    public string FullyQualifiedName { get; }
    public INavigable Navigable { get; }
    public PropertyInfo ConcreteProperty { get; }
    public bool IsOptional { get; }
    public string[] OverrideRegexPatterns { get; }
    public Type UnderlyingType { get; }
    public Type[] GenericTypes { get; }

    protected CaptureNode(Node parentNode, INavigable navigable)
        : base(parentNode, navigable.Name)
    {
        FullyQualifiedName = GetFullyQualifiedCaptureGroupName();
        Navigable = navigable;
        ConcreteProperty = (navigable as PropertySnippet)?.Prop;
        UnderlyingType = Nullable.GetUnderlyingType(navigable.Type) ?? navigable.Type;
        GenericTypes = UnderlyingType.GetGenericArguments();

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

        var value = TryGetValue(captureDictionary, out CaptureValueResult result);

        if (result != CaptureValueResult.FoundWithValue)
            return;

        ConcreteProperty.SetValue(parent, value);
    }

    public abstract object TryGetValue(CaptureDictionary captureDictionary, out CaptureValueResult result);

    public virtual object GetValueSingleCapture(Capture capture)
    {
        // todo: This is leaky because not every inheritor of CaptureNode has a way to derive a value from a single capture.
        // We may need to refactor because we don't want to silently fail by returning null, and we prefer abstract methods
        // over virtual ones for clearer compile-time warnings in inheritors who don't override the method.

        return null;
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
