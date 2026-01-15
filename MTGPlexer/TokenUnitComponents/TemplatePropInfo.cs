namespace MTGPlexer.TokenUnitComponents;

public record TemplatePropInfo
{
    public PropertyInfo Prop { get; init; }
    public RegexPropType TemplatePropType { get; init; }
    public Type BaseType { get; init; }
    public string FriendlyTypeName { get; init; }
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
        FriendlyTypeName = GetFriendlyTypeName();
        IsTerminal = CheckIsTerminal();
        Name = GetName(Prop, BaseType);
    }

    public TemplatePropInfo DeriveForManyOfItem(ManyItemOrdinal manyItemOrdinal)
    {
        if (TemplatePropType != RegexPropType.ManyOf)
            throw new Exception($"May only be derived from a ManyOf RegexPropInfo");

        var derivedManyOfPropInfo = new TemplatePropInfo
        {
            Prop = Prop,
            TemplatePropType = GetRegexPropType(BaseType),
            BaseType = BaseType,
            FriendlyTypeName = FriendlyTypeName,
            IsTerminal = IsTerminal,
            Name = manyItemOrdinal.ToString(),
        };

        return derivedManyOfPropInfo;
    }

    public TemplatePropInfo DeriveForManyOfConjunction()
    {
        if (TemplatePropType != RegexPropType.ManyOf)
            throw new Exception($"May only be derived from a ManyOf RegexPropInfo");

        var derivedConjunctionPropInfo = new TemplatePropInfo
        {
            Prop = typeof(ManyOf).GetProperty(nameof(ManyOf.Conjunction)),
            TemplatePropType = RegexPropType.ManyOfConjunction,
            BaseType = typeof(Conjunction),
            FriendlyTypeName = nameof(Conjunction).ToFriendlyCase(),
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

    static (RegexPropType, Type) GetCapturePropType(PropertyInfo prop)
    {
        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        RegexPropType regexPropType;

        if (prop.GetCustomAttribute<DistilledValueAttribute>() != null)
            regexPropType = RegexPropType.DistilledValue;
        else
            regexPropType = GetRegexPropType(type);

        if (regexPropType == RegexPropType.ManyOf || regexPropType == RegexPropType.CompoundOf || regexPropType == RegexPropType.OptionalOf)
            type = type.GetGenericArguments()[0];

        return (regexPropType, type);
    }

    string GetFriendlyTypeName()
    {
        if (TemplatePropType == RegexPropType.ManyOf)
            return "many of";

        if (TemplatePropType == RegexPropType.CompoundOf)
            return "compound of";

        if (TemplatePropType == RegexPropType.OneOf)
            return "one of";

        if (TemplatePropType == RegexPropType.OneOf)
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

    public CaptureGroupPropBase GetCaptureGroupPropBase()
    {
        return TemplatePropType switch
        {
            RegexPropType.ManyOf => new ManyOfProp(this),
            RegexPropType.CompoundOf => new CompoundOfProp(this),
            RegexPropType.OneOf => new OneOfProp(this),
            RegexPropType.OptionalOf => new OptionalOfProp(this),
            RegexPropType.TokenUnit => new TokenRegexProp(this),
            RegexPropType.TokenUnitOneOf => new TokenRegexOneOfProp(this),
            RegexPropType.Enum => new EnumRegexProp(this),
            RegexPropType.Bool => new BoolRegexProp(this),
            RegexPropType.Placeholder => new PlaceholderRegexProp(this),
            RegexPropType.Dynamic => new DynamicOfProp(this),
            _ => throw new Exception($"Prop type '{Prop.PropertyType.Name}' is not a valid RegexProp type")
        };
    }

    bool CheckIsTerminal()
    {
        List<RegexPropType> terminalTypes =
        [
            RegexPropType.Enum,
            RegexPropType.Bool,
            RegexPropType.Placeholder,
            RegexPropType.Dynamic,
            RegexPropType.DistilledValue
        ];

        return terminalTypes.Contains(TemplatePropType);
    }

    public static RegexPropType GetRegexPropType(Type type) =>
    type switch
    {
        { IsEnum: true } => RegexPropType.Enum,
        { } t when t.IsAssignableTo(typeof(ManyOf)) => RegexPropType.ManyOf,
        { } t when t.IsAssignableTo(typeof(CompoundOf)) => RegexPropType.CompoundOf,
        { } t when t.IsAssignableTo(typeof(OneOf)) => RegexPropType.OneOf,
        { } t when t.IsAssignableTo(typeof(OptionalOf)) => RegexPropType.OptionalOf,
        { } t when t == typeof(PlaceholderCapture) => RegexPropType.Placeholder,
        { } t when t.IsAssignableTo(typeof(DynamicOf)) => RegexPropType.Dynamic,
        { } t when t == typeof(bool) => RegexPropType.Bool,
        { } t when typeof(TokenUnitOneOf).IsAssignableFrom(t) => RegexPropType.TokenUnitOneOf,
        { } t when typeof(TokenUnit).IsAssignableFrom(t) => RegexPropType.TokenUnit,
        _ => throw new Exception($"{type.Name} is not a valid {nameof(TemplatePropType)} type")
    };

    public override string ToString() => Name;
}
