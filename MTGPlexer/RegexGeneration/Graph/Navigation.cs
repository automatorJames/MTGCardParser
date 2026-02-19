namespace MTGPlexer.RegexGeneration.Graph;

public class Navigation
{
    public string Name { get; private set; }
    public Type Type { get; private set; }
    public Type UnderlyingType { get; private set; }
    public Type[] GenericTypes { get; private set; }
    public Type NodeType { get; private set; }
    public string[] Patterns { get; private set; }

    // Only used for navigations to properties
    public PropertyInfo Prop { get; private set; }
    public Proptions Proptions { get; private set; } = Proptions.None;

    // Only used for navigations to TokenUnit types
    public TokenTypeConfiguration TokenTypeConfiguration { get; private set; }

    // Convenience bools to simplify logic in Node constructors
    public bool IsTokenUnitType { get; private set; }
    public bool IsRoot { get; private set; }
    public bool IsList { get; private set; }

    public Navigation(Type type)
    {
        SetTypeInfo(type);

        if (!IsTokenUnitType)
            throw new Exception($"This constructor may only be used for {nameof(TokenUnit)} types");

        IsRoot = true;
        Name = UnderlyingType.Name;
        Patterns = Type.GetCustomAttribute<RegexPatternAttribute>()?.Patterns;
    }

    public Navigation(PropertySnippet propertySnippet)
    {
        SetTypeInfo(propertySnippet.Type);
        Name = propertySnippet.Name;
        Patterns = propertySnippet.Prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns;
        Prop = propertySnippet.Prop;
        Proptions = propertySnippet.Proptions;
    }

    void SetTypeInfo(Type type)
    {
        Type = type;
        UnderlyingType = (Nullable.GetUnderlyingType(type) ?? type);
        GenericTypes = UnderlyingType.GenericTypeArguments;
        IsList = UnderlyingType.IsGenericType && UnderlyingType.GetGenericTypeDefinition() == typeof(List<>);
        NodeType = IsList ? GenericTypes[0] : UnderlyingType;
        IsTokenUnitType = NodeType.IsAssignableTo(typeof(TokenUnit));

        if (NodeType.IsAssignableTo(typeof(TokenUnit)))
            TokenTypeConfiguration = TokenTypeRegistry.GetTokenUnitTypeConfiguration(NodeType);
    }

    public override string ToString() => Name;
}
