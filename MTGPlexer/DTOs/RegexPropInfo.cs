namespace MTGPlexer.DTOs;

/// <summary>
/// Records property information specific to the domain of this library. This record is used by processes
/// that need to distinguish among domain-relevent property types (enums, bools, text placeholders, and child token units).
/// Offers conveniences such as exposing attribute-defined Regex patterns.
/// </summary>
public record RegexPropInfo
{
    public PropertyInfo Prop { get; }
    public RegexPropType RegexPropType { get; }
    public Type UnderlyingType { get; }
    public string Name { get; }
    public string[] AttributePatterns { get; }
    public PropertyInfo UndistilledProp { get; }
    public string FriendlyTypeName { get; }
    public string FriendlyPropName { get; }
    //public string UndistilledNameOrName => UndistilledProp?.Name ?? Name;
    //public string UndistilledNameOrNameFriendly => UndistilledNameOrName.ToFriendlyCase(TitleDisplayOption.Sentence);

    public RegexPropInfo(PropertyInfo prop)
    {
        //// If this prop is distilled from another one, get the other one
        //var distilledValAttribute = prop.GetCustomAttribute<DistilledValueAttribute>();
        //if (distilledValAttribute != null)
        //{
        //    prop = distilledValAttribute.GetDistilledFromProp(prop);
        //    UndistilledProp = prop;
        //}

        Prop = prop;
        RegexPropType = GetCapturePropType(prop);
        UnderlyingType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        Name = prop.Name;
        AttributePatterns = prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns ?? [prop.Name];
        FriendlyPropName = prop.Name.ToFriendlyCase(TitleDisplayOption.Sentence);
        FriendlyTypeName = GetFriendlyTypeName(UnderlyingType);
    }

    RegexPropType GetCapturePropType(PropertyInfo prop)
    {
        var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        if (underlyingType.IsEnum)
            return RegexPropType.Enum;
        else if (underlyingType == typeof(PlaceholderCapture))
            return RegexPropType.Placeholder;
        else if (underlyingType == typeof(bool))
            return RegexPropType.Bool;
        else if (underlyingType.IsAssignableTo(typeof(TokenUnitOneOf)))
            return RegexPropType.TokenUnitOneOf;
        else if (underlyingType.IsAssignableTo(typeof(TokenUnit)))
            return RegexPropType.TokenUnit;
        else if (prop.GetCustomAttribute<DistilledValueAttribute>() != null)
            return RegexPropType.DistilledValue;
        else
            throw new Exception($"{prop.PropertyType.Name} is not a valid {nameof(RegexPropType)} type");
    }

    string GetFriendlyTypeName(Type type)
    {
        bool isNullableEnum = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && type.GetGenericArguments()[0].IsEnum;
        if (type.IsEnum || isNullableEnum) return "Enum";
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) return $"{type.GetGenericArguments()[0].Name}?";
        if (type.IsAssignableTo(typeof(TokenUnit))) return "Token Unit";
        return type.Name;
    }

}

