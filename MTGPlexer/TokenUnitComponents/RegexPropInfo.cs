namespace MTGPlexer.TokenUnitComponents;

public record RegexPropInfo
{
    public PropertyInfo Prop { get; init; }
    public RegexPropType RegexPropType { get; init; }
    public bool IsManyOfProp { get; init; }
    public Type BaseType { get; init; }
    public Type UnderlyingType { get; init; }
    public string FriendlyTypeName { get; init; }
    public string FriendlyPropName { get; init; }
    public bool IsTerminal { get; init; }
    public bool MayBeNull { get; init; }
    public string Name { get; init; }
    public ManyItemOrdinal? ManyItemOrdinal { get; init; }

    /// <summary>
    /// If not null, any recursive descendants of this RegexPropInfo will inherit this
    /// appendix, which may grow as the descendancy tree grows. Used for scenarios like
    /// ManyOf items, which must pass their distinguishing ordinal name down to all children
    /// </summary>
    public string ManyOfItemDistinguisher { get; init; }

    private RegexPropInfo()
    {
    }

    public RegexPropInfo(PropertyInfo prop)
    {
        var nullableType = Nullable.GetUnderlyingType(prop.PropertyType);
        Prop = prop;
        (RegexPropType, IsManyOfProp, BaseType) = GetCapturePropType(prop);
        UnderlyingType = nullableType ?? prop.PropertyType;
        Name = prop.Name;
        FriendlyPropName = prop.Name.ToFriendlyCase(TitleDisplayOption.Sentence);
        FriendlyTypeName = GetFriendlyTypeName();
        IsTerminal = CheckIsTerminal();
        MayBeNull = nullableType != null;
    }

    public RegexPropInfo DerviveForManyOfItem(ManyItemOrdinal manyItemOrdinal)
    {
        if (!IsManyOfProp)
            throw new Exception($"May only derive for a {nameof(RegexPropInfo)} ManyOf item instance if {nameof(IsManyOfProp)} is true");

        var derivedManyOfPropInfo = new RegexPropInfo
        {
            Prop = Prop,
            RegexPropType = BaseType.GetRegexPropType(),
            BaseType = BaseType,
            UnderlyingType = UnderlyingType,
            FriendlyTypeName = FriendlyTypeName,
            FriendlyPropName = FriendlyPropName,
            IsTerminal = IsTerminal,
            MayBeNull = MayBeNull,
            IsManyOfProp = false, // "IsManyOfProp" only refers to the parent ManyOf, not the items it contains
            Name = manyItemOrdinal.ToString(),
            ManyOfItemDistinguisher = manyItemOrdinal.ToString(),
            ManyItemOrdinal = manyItemOrdinal
        };

        return derivedManyOfPropInfo;
    }

    public RegexPropInfo DerviveForManyOfConjunction()
    {
        if (!IsManyOfProp)
            throw new Exception($"May only derive for a {nameof(RegexPropInfo)} ManyOf {nameof(ManyOf.Conjunction)} if {nameof(IsManyOfProp)} is true");

        var derivedManyOfPropInfo = new RegexPropInfo
        {
            Prop = typeof(ManyOf).GetProperty(nameof(ManyOf.Conjunction)),
            RegexPropType = RegexPropType.ManyOfConjunction,
            BaseType = typeof(Conjunction),
            UnderlyingType = typeof(Conjunction),
            FriendlyTypeName = nameof(Conjunction).ToFriendlyCase(),
            FriendlyPropName = nameof(Conjunction).ToFriendlyCase(),
            IsTerminal = true,
            MayBeNull = true,
            IsManyOfProp = false,
            Name = nameof(Conjunction)
        };

        return derivedManyOfPropInfo;
    }


    static (RegexPropType, bool, Type) GetCapturePropType(PropertyInfo prop)
    {
        Type type = prop.PropertyType;
        bool isArray = false;

        if (type.IsArray)
        {
            isArray = true;
            type = type.GetElementType()!;
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ManyOf<>))
        {
            isArray = true;
            type = type.GetGenericArguments()[0];
        }

        type = Nullable.GetUnderlyingType(type) ?? type;

        RegexPropType regexPropType;

        if (isArray)
            regexPropType = RegexPropType.ManyOf;
        else if (prop.GetCustomAttribute<DistilledValueAttribute>() != null)
            regexPropType = RegexPropType.DistilledValue;
        else
            regexPropType = type.GetRegexPropType();

        return (regexPropType, isArray, type);
    }

    string GetFriendlyTypeName()
    {
        if (IsManyOfProp)
            return "many of";

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

    public CaptureGroupPropBase GetCaptureGroupPropBase(bool forceGetUnderlyingPropType = false)
    {
        if (IsManyOfProp && !forceGetUnderlyingPropType)
            return new TokenRegexManyProp(this);

        return RegexPropType switch
        {
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
    
    public override string ToString() => Name;
}
