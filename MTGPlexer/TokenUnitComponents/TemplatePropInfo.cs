namespace MTGPlexer.TokenUnitComponents;

public record TemplatePropInfo
{
    public PropertyInfo Prop { get; init; }
    public TemplatePropType TemplatePropType { get; init; }
    public Type BaseType { get; init; }
    public bool IsTerminal { get; init; }
    public string Name { get; init; }

    private TemplatePropInfo()
    {
    }

    public TemplatePropInfo(PropertyInfo prop)
    {
        var nullableType = Nullable.GetUnderlyingType(prop.PropertyType);
        Prop = prop;
        (TemplatePropType, BaseType) = GetCapturePropType(prop);
        IsTerminal = CheckIsTerminal();
        Name = GetName(Prop, BaseType);
    }

    public TemplatePropInfo DeriveForManyOfItem(ManyItemOrdinal manyItemOrdinal)
    {
        if (TemplatePropType != TemplatePropType.ManyOf)
            throw new Exception($"May only be derived from a ManyOf TemplatePropInfo");

        var derivedManyOfPropInfo = new TemplatePropInfo
        {
            Prop = Prop,
            TemplatePropType = GetRegexPropType(BaseType),
            BaseType = BaseType,
            IsTerminal = IsTerminal,
            Name = manyItemOrdinal.ToString(),
        };

        return derivedManyOfPropInfo;
    }

    public TemplatePropInfo DeriveForManyOfConjunction()
    {
        if (TemplatePropType != TemplatePropType.ManyOf)
            throw new Exception($"May only be derived from a ManyOf TemplatePropInfo");

        var derivedConjunctionPropInfo = new TemplatePropInfo
        {
            Prop = typeof(ManyOf).GetProperty(nameof(ManyOf.Conjunction)),
            TemplatePropType = TemplatePropType.ManyOfConjunction,
            BaseType = typeof(Conjunction),
            IsTerminal = true,
            Name = nameof(Conjunction)
        };

        return derivedConjunctionPropInfo;
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

    static (TemplatePropType, Type) GetCapturePropType(PropertyInfo prop)
    {
        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        TemplatePropType regexPropType;

        if (prop.GetCustomAttribute<DistilledValueAttribute>() != null)
            regexPropType = TemplatePropType.DistilledValue;
        else
            regexPropType = GetRegexPropType(type);

        if (regexPropType == TemplatePropType.ManyOf || regexPropType == TemplatePropType.CompoundOf || regexPropType == TemplatePropType.OptionalOf)
            type = type.GetGenericArguments()[0];

        return (regexPropType, type);
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

        bool isNullableEnum = BaseType.IsGenericType && BaseType.GetGenericTypeDefinition() == typeof(Nullable<>) && BaseType.GetGenericArguments()[0].IsEnum;

        if (BaseType.IsEnum || isNullableEnum)
            return "enum";

        if (BaseType.IsGenericType && BaseType.GetGenericTypeDefinition() == typeof(Nullable<>))
            return $"{BaseType.GetGenericArguments()[0].Name}".ToFriendlyCase(TitleDisplayOption.Sentence);

        if (BaseType == typeof(int))
            return "int";

        if (BaseType == typeof(PlaceholderCapture))
            return "placeholder";

        if (BaseType.IsAssignableTo(typeof(DynamicOf)))
            return "dynamic";

        if (BaseType.IsAssignableTo(typeof(TokenUnitOneOf)))
            return "one of";

        if (BaseType.IsAssignableTo(typeof(TokenUnit)))
            return "token unit";

        return BaseType.Name.ToFriendlyCase(TitleDisplayOption.Sentence).ToLower();
    }

    public CaptureGroupSegmentBase GetCaptureGroupPropBase()
    {
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

    bool CheckIsTerminal()
    {
        List<TemplatePropType> terminalTypes =
        [
            TemplatePropType.Enum,
            TemplatePropType.Bool,
            TemplatePropType.Placeholder,
            TemplatePropType.Dynamic,
            TemplatePropType.DistilledValue
        ];

        return terminalTypes.Contains(TemplatePropType);
    }

    public static TemplatePropType GetRegexPropType(Type type) =>
    type switch
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