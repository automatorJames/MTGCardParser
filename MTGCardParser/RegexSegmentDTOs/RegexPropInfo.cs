namespace MTGCardParser.RegexSegmentDTOs;

/// <summary>
/// Records property information specific to the domain of this library. This record is used by processes
/// that need to distinguish among domain-relevent property types (enums, bools, text placeholders, and child token units).
/// Offers conveniences such as exposing attribute-defined Regex patterns, if any.
/// </summary>
public record RegexPropInfo
{
    public PropertyInfo Prop { get; init; }
    public RegexPropType CapturePropType { get; init; }
    public Type UnderlyingType { get; init; }
    public string Name { get; init; }
    public string[] AttributePatterns { get; init; }

    public RegexPropInfo(PropertyInfo prop)
    {
        Prop = prop;
        CapturePropType = GetCapturePropType(prop);
        UnderlyingType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        Name = prop.Name;
        AttributePatterns = prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns ?? [Prop.Name];
    }

    RegexPropType GetCapturePropType(PropertyInfo prop)
    {
        var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        if (underlyingType.IsEnum)
            return RegexPropType.Enum;
        else if (underlyingType == typeof(PlaceholderCapture))
            return RegexPropType.TextPlaceholder;
        else if (underlyingType == typeof(bool))
            return RegexPropType.Bool;
        else if (underlyingType.IsAssignableTo(typeof(TokenUnit)))
            return RegexPropType.TokenUnit;
        else
            throw new Exception($"{prop.PropertyType.Name} is not a valid {nameof(CapturePropType)} type");
    }
}

