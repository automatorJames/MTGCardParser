namespace MTGCardParser;

/// <summary>
/// A unified definition for a capturable property on a TokenUnit.
/// It knows how to generate its regex segment and how to hydrate its
/// corresponding property from a regex Match. This replaces the IRegexSegment hierarchy.
/// </summary>
public abstract record PropertyCapture
{
    public PropertyInfo Prop { get; }
    public string CaptureGroupName => Prop.Name;
    public abstract string RegexPattern { get; }

    protected PropertyCapture(PropertyInfo prop)
    {
        Prop = prop;
    }

    public abstract void HydrateProperty(TokenUnit instance, Match match);

    public static PropertyCapture Create(PropertyInfo prop)
    {
        var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        if (underlyingType.IsEnum)
            return new EnumPropertyCapture(prop);
        if (underlyingType == typeof(bool))
            return new BoolPropertyCapture(prop);
        if (underlyingType.IsAssignableTo(typeof(TokenUnit)))
            return new TokenUnitPropertyCapture(prop);
        if (underlyingType == typeof(CapturedTextSegment))
            return new TextSegmentPropertyCapture(prop);

        throw new NotSupportedException($"Property '{prop.Name}' with type '{prop.PropertyType.Name}' is not a valid capturable type.");
    }

    public static bool IsValidCaptureProperty(PropertyInfo prop)
    {
        var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        return underlyingType.IsEnum ||
               underlyingType == typeof(bool) ||
               underlyingType.IsAssignableTo(typeof(TokenUnit)) ||
               underlyingType == typeof(CapturedTextSegment);
    }

    protected TextSpan? GetSubSpanFromGroup(TextSpan originalSpan, Group group)
    {
        if (!group.Success) return null;

        var combinedIndex = originalSpan.Position.Absolute + group.Index;
        var newPosition = new Position(combinedIndex, originalSpan.Position.Line, combinedIndex + 1);
        return new TextSpan(originalSpan.Source, newPosition, group.Length);
    }
}