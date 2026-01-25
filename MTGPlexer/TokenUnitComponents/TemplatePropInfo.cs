namespace MTGPlexer.TokenUnitComponents;

public record TemplatePropInfo
{
    public PropertyInfo Prop { get; init; }
    public TemplatePropType TemplatePropType { get; init; }
    public Type UnderlyingType { get; init; }
    public Type[] GenericTypes { get; init; }
    public bool IsTerminal { get; init; }
    public string Name { get; init; }
    public Proptions Proptions { get; set; } = Proptions.None;

    private TemplatePropInfo()
    {
    }

    public TemplatePropInfo(PropertyInfo prop)
    {
        var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        UnderlyingType = underlyingType;
        GenericTypes = underlyingType.GetGenericArguments();
        Prop = prop;
        TemplatePropType = GetTemplatePropType(underlyingType, Prop);
        IsTerminal = _terminalTypes.Contains(TemplatePropType);
        Name = GetName(Prop, underlyingType);
    }

    public TemplatePropInfo DeriveForXOfItem(string name = null, int genericTypeIndex = 0)
    {
        if (!UnderlyingType.IsAssignableTo(typeof(XOf)))
            throw new Exception($"May only be derived from XOf types");

        if (GenericTypes.Length <= genericTypeIndex)
            throw new IndexOutOfRangeException();

        var genericType = GenericTypes[genericTypeIndex];
        name ??= genericType.Name;
        var templatePropType = GetTemplatePropType(genericType);

        return new TemplatePropInfo
        {
            Prop = typeof(PolyItemCapture).GetProperty(nameof(PolyItemCapture.Value)),
            TemplatePropType = templatePropType,
            UnderlyingType = genericType,
            GenericTypes = genericType.GetGenericArguments(),
            IsTerminal = _terminalTypes.Contains(templatePropType),
            Name = name
        };
    }

    public TemplatePropInfo DeriveForManyOfConjunction()
    {
        if (TemplatePropType != TemplatePropType.ManyOf)
            throw new Exception($"May only be derived from a ManyOf TemplatePropInfo");

        var derivedConjunctionPropInfo = new TemplatePropInfo
        {
            Prop = typeof(ManyOf).GetProperty(nameof(ManyOf.Conjunction)),
            TemplatePropType = TemplatePropType.ManyOfConjunction,
            UnderlyingType = typeof(Conjunction),
            IsTerminal = true,
            Name = nameof(Conjunction)
        };

        return derivedConjunctionPropInfo;
    }

    /// <summary>
    /// Constructor used for generating "regex previews" from snippets, some of which may be type names, and
    /// which require conversion to a RegexSegment. In this workflow, the "Prop" property is never intended
    /// to be used to set a value on any object.
    /// </summary>
    public TemplatePropInfo(Type type, string name)
    {
        // Here, "Prop" is a Dummy placeholder that is never intended to be used to set a value.
        // We assign a Prop merely so we don't break downstream code that checks things like nullability
        // of Prop.
        Prop = GetType().GetProperty(nameof(Prop));

        TemplatePropType = GetTemplatePropType(type);
        UnderlyingType = type;
        GenericTypes = type.GetGenericArguments();
        IsTerminal = _terminalTypes.Contains(TemplatePropType);
        Name = name;
    }

    static string GetName(PropertyInfo prop, Type underlyingType)
    {
        if (underlyingType.IsAssignableTo(typeof(CompoundOf)))
            return nameof(CompoundOf) + prop.Name;

        if (underlyingType.IsAssignableTo(typeof(ManyOf)))
            return nameof(ManyOf) + prop.Name;

        if (underlyingType.IsAssignableTo(typeof(OneOf)))
            return nameof(OneOf) + prop.Name;

        if (underlyingType.IsAssignableTo(typeof(ManyOf)))
            return nameof(OptionalOf) + prop.Name;

        return prop.Name;
    }

    public string GetFriendlyTypeName()
    {
        if (TemplatePropType == TemplatePropType.ManyOfConjunction)
            return "conjunction";

        if (TemplatePropType == TemplatePropType.ManyOf)
            return "many of";

        if (TemplatePropType == TemplatePropType.CompoundOf)
            return "compound of";

        if (TemplatePropType == TemplatePropType.OneOf)
            return "one of";

        if (TemplatePropType == TemplatePropType.OneOf)
            return "optional of";

        // Assign to variable to avoid excessive reads on property, which muddy up reference counts
        var underlyingType = UnderlyingType;

        bool isNullableEnum = underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(Nullable<>) && underlyingType.GetGenericArguments()[0].IsEnum;

        if (underlyingType.IsEnum || isNullableEnum)
            return "enum";

        if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(Nullable<>))
            return $"{UnderlyingType.GetGenericArguments()[0].Name}".ToFriendlyCase(TitleDisplayOption.Sentence);

        if (underlyingType == typeof(int))
            return "int";

        if (underlyingType == typeof(PlaceholderCapture))
            return "placeholder";

        if (underlyingType.IsAssignableTo(typeof(DynamicOf)))
            return "dynamic";

        if (underlyingType.IsAssignableTo(typeof(TokenUnitOneOf)))
            return "one of";

        if (underlyingType.IsAssignableTo(typeof(TokenUnit)))
            return "token unit";

        return underlyingType.Name.ToFriendlyCase(TitleDisplayOption.Sentence).ToLower();
    }

    public CaptureGroupSegmentBase GetCaptureGroupPropBase(Proptions proptions = Proptions.None)
    {
        this.Proptions = proptions;

        return TemplatePropType switch
        {
            TemplatePropType.ManyOf => new ManyOfSegment(this),
            TemplatePropType.CompoundOf => new CompoundOfSegment(this),
            TemplatePropType.OneOf => new OneOfSegment(this),
            TemplatePropType.OptionalOf => new OptionalOfSegment(this),
            TemplatePropType.TokenUnit => new TokenUnitSegment(this),
            TemplatePropType.TokenUnitOneOf => new TokenUnitOneOfSegment(this),
            TemplatePropType.Enum => new EnumSegment(this),
            TemplatePropType.Bool => new BoolSegment(this),
            TemplatePropType.Placeholder => new PlaceholderSegment(this),
            TemplatePropType.Dynamic => new DynamicOfSegment(this),
            _ => throw new Exception($"Prop type '{Prop.PropertyType.Name}' is not a valid RegexProp type")
        };
    }

    public static TemplatePropType GetTemplatePropType(Type type, PropertyInfo prop = null)
    {
        if (prop?.GetCustomAttribute<DistilledValueAttribute>() != null)
            return TemplatePropType.DistilledValue;

        return type switch
        {
            { IsEnum: true } => TemplatePropType.Enum,
            { } t when t.IsAssignableTo(typeof(ManyOf)) => TemplatePropType.ManyOf,
            { } t when t.IsAssignableTo(typeof(CompoundOf)) => TemplatePropType.CompoundOf,
            { } t when t.IsAssignableTo(typeof(OneOf)) => TemplatePropType.OneOf,
            { } t when t.IsAssignableTo(typeof(OptionalOf)) => TemplatePropType.OptionalOf,
            { } t when t == typeof(PlaceholderCapture) => TemplatePropType.Placeholder,
            { } t when t.IsAssignableTo(typeof(DynamicOf)) => TemplatePropType.Dynamic,
            { } t when t == typeof(bool) => TemplatePropType.Bool,
            { } t when typeof(TokenUnitOneOf).IsAssignableFrom(t) => TemplatePropType.TokenUnitOneOf,
            { } t when typeof(TokenUnit).IsAssignableFrom(t) => TemplatePropType.TokenUnit,
            _ => throw new Exception($"{type.Name} is not a valid {nameof(TemplatePropType)} type")
        };
    }

    static HashSet<TemplatePropType> _terminalTypes =
    [
        TemplatePropType.Enum,
        TemplatePropType.Bool,
        TemplatePropType.Placeholder,
        TemplatePropType.Dynamic,
        TemplatePropType.DistilledValue
    ];

    public override string ToString() => Name;
}

public enum TemplatePropType
{
    Enum,
    Placeholder,
    Dynamic,
    Bool,
    DistilledValue,
    TokenUnit,
    TokenUnitOneOf,
    ManyOf,
    ManyOfConjunction,
    CompoundOf,
    OneOf,
    OptionalOf,
}