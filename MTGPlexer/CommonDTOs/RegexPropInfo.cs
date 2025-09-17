using MTGPlexer.RegexGeneration.RegexSegments;

namespace MTGPlexer.CommonDTOs;

public record RegexPropInfo
{
    public PropertyInfo Prop { get; }
    public RegexPropType RegexPropType { get; }
    public bool IsManyItem { get; }
    public Type BaseType { get; }
    public Type UnderlyingType { get; }
    public string Name { get; }
    public string FriendlyTypeName { get; }
    public string FriendlyPropName { get; }
    public bool IsTerminal { get; }
    public bool MayBeNull { get; }

    public RegexPropInfo(PropertyInfo prop)
    {
        var nullableType = Nullable.GetUnderlyingType(prop.PropertyType);
        Prop = prop;
        (RegexPropType, IsManyItem, BaseType) = GetCapturePropType(prop);
        UnderlyingType = nullableType ?? prop.PropertyType;
        Name = prop.Name;
        FriendlyPropName = prop.Name.ToFriendlyCase(TitleDisplayOption.Sentence);
        FriendlyTypeName = GetFriendlyTypeName();
        IsTerminal = CheckIsTerminal();
        MayBeNull = nullableType != null;
    }

    private static (RegexPropType, bool, Type) GetCapturePropType(PropertyInfo prop)
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

        RegexPropType regexPropType =
            type.IsEnum ? RegexPropType.Enum :
            type == typeof(PlaceholderCapture) ? RegexPropType.Placeholder :
            type.IsAssignableTo(typeof(DynamicCapture)) ? RegexPropType.Dynamic :
            type == typeof(bool) ? RegexPropType.Bool :
            typeof(TokenUnitOneOf).IsAssignableFrom(type) ? RegexPropType.TokenUnitOneOf :
            typeof(TokenUnit).IsAssignableFrom(type) ? RegexPropType.TokenUnit :
            prop.GetCustomAttribute<DistilledValueAttribute>() != null ? RegexPropType.DistilledValue :
            throw new Exception($"{prop.PropertyType.Name} is not a valid {nameof(RegexPropType)} type");

        return (regexPropType, isArray, type);
    }

    string GetFriendlyTypeName()
    {
        if (IsManyItem)
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
        if (IsManyItem && !forceGetUnderlyingPropType)
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
            RegexPropType.DistilledValue
        ];

        return terminalTypes.Contains(RegexPropType);
    }
}
