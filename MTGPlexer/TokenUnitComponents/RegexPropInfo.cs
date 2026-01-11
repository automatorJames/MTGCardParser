namespace MTGPlexer.TokenUnitComponents;

public record RegexPropInfo
{
    public PropertyInfo Prop { get; init; }
    public RegexPropType RegexPropType { get; init; }
    public Type BaseType { get; init; }
    public string FriendlyTypeName { get; init; }
    public string FriendlyPropName { get; init; }
    public bool IsTerminal { get; init; }
    public bool MayBeNull { get; init; }
    public string Name { get; init; }

    /// <summary>
    /// If not null, any recursive descendants of this RegexPropInfo will inherit this
    /// appendix, which may grow as the descendancy tree grows. Used for scenarios like
    /// ManyOf items, which must pass their distinguishing ordinal name down to all children
    /// </summary>
    public string ItemDistinguisher { get; init; }

    private RegexPropInfo()
    {
    }

    public RegexPropInfo(PropertyInfo prop)
    {
        var nullableType = Nullable.GetUnderlyingType(prop.PropertyType);
        Prop = prop;
        (RegexPropType, BaseType) = GetCapturePropType(prop);
        FriendlyPropName = prop.Name.ToFriendlyCase(TitleDisplayOption.Sentence);
        FriendlyTypeName = GetFriendlyTypeName();
        IsTerminal = CheckIsTerminal();
        MayBeNull = nullableType != null;
        Name = GetName(Prop, BaseType);
    }

    public RegexPropInfo DeriveForManyOfItem(ManyItemOrdinal manyItemOrdinal)
    {
        if (RegexPropType != RegexPropType.ManyOf)
            throw new Exception($"May only be derived from a ManyOf RegexPropInfo");

        var derivedManyOfPropInfo = new RegexPropInfo
        {
            Prop = Prop,
            RegexPropType = GetRegexPropType(BaseType),
            BaseType = BaseType,
            FriendlyTypeName = FriendlyTypeName,
            FriendlyPropName = FriendlyPropName,
            IsTerminal = IsTerminal,
            MayBeNull = MayBeNull,
            Name = manyItemOrdinal.ToString(),
            ItemDistinguisher = manyItemOrdinal.ToString(),
        };

        return derivedManyOfPropInfo;
    }

    public RegexPropInfo DeriveForManyOfConjunction()
    {
        if (RegexPropType != RegexPropType.ManyOf)
            throw new Exception($"May only be derived from a ManyOf RegexPropInfo");

        var derivedConjunctionPropInfo = new RegexPropInfo
        {
            Prop = typeof(ManyOf).GetProperty(nameof(ManyOf.Conjunction)),
            RegexPropType = RegexPropType.ManyOfConjunction,
            BaseType = typeof(Conjunction),
            FriendlyTypeName = nameof(Conjunction).ToFriendlyCase(),
            FriendlyPropName = nameof(Conjunction).ToFriendlyCase(),
            IsTerminal = true,
            MayBeNull = true,
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

        if (regexPropType == RegexPropType.ManyOf || regexPropType == RegexPropType.CompoundOf)
            type = type.GetGenericArguments()[0];

        return (regexPropType, type);
    }

    string GetFriendlyTypeName()
    {
        if (RegexPropType == RegexPropType.ManyOf)
            return "many of";

        if (RegexPropType == RegexPropType.CompoundOf)
            return "compound of";

        if (RegexPropType == RegexPropType.OneOf)
            return "one of";

        bool isNullableEnum = BaseType.IsGenericType && BaseType.GetGenericTypeDefinition() == typeof(Nullable<>) && BaseType.GetGenericArguments()[0].IsEnum;

        if (BaseType.IsEnum || isNullableEnum)
            return "enum";

        if (BaseType.IsGenericType && BaseType.GetGenericTypeDefinition() == typeof(Nullable<>))
            return $"{BaseType.GetGenericArguments()[0].Name}".ToFriendlyCase(TitleDisplayOption.Sentence);

        if (BaseType == typeof(int))
            return "int";

        if (BaseType == typeof(PlaceholderCapture))
            return "placeholder";

        if (BaseType.IsAssignableTo(typeof(DynamicCapture)))
            return "dynamic";

        if (BaseType.IsAssignableTo(typeof(TokenUnitOneOf)))
            return "one of";

        if (BaseType.IsAssignableTo(typeof(TokenUnit)))
            return "token unit";

        return BaseType.Name.ToFriendlyCase(TitleDisplayOption.Sentence).ToLower();
    }

    public CaptureGroupPropBase GetCaptureGroupPropBase()
    {
        return RegexPropType switch
        {
            RegexPropType.ManyOf => new ManyProp(this),
            RegexPropType.CompoundOf => new CompoundProp(this),
            RegexPropType.OneOf => new OneOfProp(this),
            RegexPropType.TokenUnit => new TokenRegexProp(this),
            RegexPropType.TokenUnitOneOf => new TokenRegexOneOfProp(this),
            RegexPropType.Enum => new EnumRegexProp(this),
            RegexPropType.Bool => new BoolRegexProp(this),
            RegexPropType.Placeholder => new PlaceholderRegexProp(this),
            RegexPropType.Dynamic => new DynamicRegexProp(this),
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

        return terminalTypes.Contains(RegexPropType);
    }

    public static RegexPropType GetRegexPropType(Type type) =>
    type switch
    {
        { IsEnum: true } => RegexPropType.Enum,
        { } t when t.IsAssignableTo(typeof(ManyOf)) => RegexPropType.ManyOf,
        { } t when t.IsAssignableTo(typeof(CompoundOf)) => RegexPropType.CompoundOf,
        { } t when t.IsAssignableTo(typeof(OneOf)) => RegexPropType.OneOf,
        { } t when t == typeof(PlaceholderCapture) => RegexPropType.Placeholder,
        { } t when t.IsAssignableTo(typeof(DynamicCapture)) => RegexPropType.Dynamic,
        { } t when t == typeof(bool) => RegexPropType.Bool,
        { } t when typeof(TokenUnitOneOf).IsAssignableFrom(t) => RegexPropType.TokenUnitOneOf,
        { } t when typeof(TokenUnit).IsAssignableFrom(t) => RegexPropType.TokenUnit,
        _ => throw new Exception($"{type.Name} is not a valid {nameof(RegexPropType)} type")
    };

    public override string ToString() => Name;
}
