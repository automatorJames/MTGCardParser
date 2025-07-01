using MTGCardParser.TokenUnits.Interfaces;

namespace MTGCardParser.RegexSegmentDTOs;

public record CaptureProp
{
    public PropertyInfo Prop { get; init; }
    public CapturePropType CapturePropType { get; init; }
    public Type UnderlyingType { get; init; }
    public string Name { get; init; }
    public string[] AttributePatterns { get; init; }

    public CaptureProp(PropertyInfo prop)
    {
        Prop = prop;
        CapturePropType = GetCapturePropType(prop);
        UnderlyingType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        Name = prop.Name;
        AttributePatterns = prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns ?? [Prop.Name];
    }

    CapturePropType GetCapturePropType(PropertyInfo prop)
    {
        var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        if (underlyingType.IsEnum)
            return CapturePropType.Enum;
        else if (underlyingType == typeof(CapturedTextSegment))
            return CapturePropType.CapturedTextSegment;
        else if (underlyingType == typeof(bool))
            return CapturePropType.Bool;
        else if (underlyingType.IsAssignableTo(typeof(ITokenUnit)))
            return CapturePropType.TokenUnit;
        else
            throw new Exception($"{prop.PropertyType.Name} is not a valid {nameof(CapturePropType)} type");
    }
}

