
namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class GroupNode : RegexNode
{
    public virtual GroupQuantifier? Quantifier => null;

    public CaptureValueHydrationInfo CaptureValueHydrationInfo { get; protected set; }
    public string FullyQualifiedName { get; }
    public INavigable Navigable { get; }
    public PropertyInfo ConcreteProperty { get; }
    public bool IsOptional { get; }
    public string[] OverrideRegexPatterns { get; }
    public Type UnderlyingType { get; }
    public Type[] GenericTypes { get; }

    protected virtual bool AbortIfSetPropertyToNull => false;

    protected GroupNode(RegexNode parentNode, INavigable navigable)
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

    public abstract object GetValueAndSetHydrationInfo(CaptureContext captureContext);

    string GetFullyQualifiedCaptureGroupName()
    {
        List<string> parts = [];
        RegexNode current = this;

        while (current != null)
        {
            if (!current.IsCollapsible)
                parts.Add(current.Name);

            current = current.ParentNode;
        }

        parts.Reverse();
        parts = parts.Skip(1).ToList();
        return string.Join('_', parts);
    }

    public GroupNode(RegexNode parentNode, string name) : base(parentNode, name)
    {
    }
}
