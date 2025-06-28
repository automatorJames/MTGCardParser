namespace MTGCardParser;

public record CaptureProp
{
    public PropertyInfo Prop { get; init; }
    public Type UnderlyingPropType { get; init; }
    public CapturePropType CapturePropType { get; set; }

    public CaptureProp(PropertyInfo prop)
    {
        Prop = prop;
        CapturePropType = GetCapturePropType(prop);
        UnderlyingPropType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
    }

    public void SetValue(object parentInstance, Match match)
    {
        if (!match.Groups.ContainsKey(Prop.Name))
            throw new Exception($"No capture group defined in {nameof(RegexTemplate)} for type {Prop.DeclaringType.Name} that maches property name {Prop.Name}");

        var matchString = match.Groups[Prop.Name].Value;

        var valueToSet = CapturePropType switch
        {
            CapturePropType.Enum => GetEnumMatchValue(matchString),
            CapturePropType.TokenSegment => new TokenSegment(matchString),
            CapturePropType.Bool => !string.IsNullOrEmpty(matchString),
            CapturePropType.TokenCapture => TypeRegistry.InstantiateFromTypeAndMatchString(Prop.PropertyType, matchString),
        };

        Prop.SetValue(parentInstance, valueToSet);
    }

    object GetEnumMatchValue(string matchString)
    {
        if (!TypeRegistry.EnumRegexes.ContainsKey(UnderlyingPropType))
            throw new Exception($"Enum type {UnderlyingPropType.Name} is not registered in {nameof(TypeRegistry)}");

        foreach (var enumMemberRegex in TypeRegistry.EnumRegexes[UnderlyingPropType])
            if (enumMemberRegex.Value.IsMatch(matchString))
                return enumMemberRegex.Key;

        return null;
    }

    static CapturePropType GetCapturePropType(PropertyInfo prop)
    {
        var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        if (underlyingType.IsEnum)
            return CapturePropType.Enum;
        else if (underlyingType == typeof(TokenSegment))
            return CapturePropType.TokenSegment;
        else if (underlyingType == typeof(bool))
            return CapturePropType.Bool;
        else if (underlyingType.IsAssignableTo(typeof(ITokenCapture)))
            return CapturePropType.TokenCapture;
        else
            throw new Exception($"{prop.PropertyType.Name} is not a valid {nameof(CaptureProp)} type");
    }
}

public enum CapturePropType
{
    Enum,
    TokenSegment,
    Bool,
    TokenCapture
}

