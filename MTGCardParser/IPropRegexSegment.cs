namespace MTGCardParser;

public abstract record PropRegexSegmentBase : RegexSegmentBase, IPropRegexSegment
{
    public CaptureProp CaptureProp { get; init; }
    public bool IsChildTokenUnit => CaptureProp.CapturePropType == CapturePropType.TokenUnit;
    
    public PropRegexSegmentBase(CaptureProp captureProp)
    {
        CaptureProp = captureProp;
    }

    public void SetValueFromMatchString(object parentObject, string matchString)
    {
        if (IsChildTokenUnit)
            SetChildTokenUnitValue(parentObject, matchString);
        else
            SetScalarPropValue(parentObject, matchString);
    }

    void SetScalarPropValue(object parentObject, string matchString)
    {
        if (IsChildTokenUnit)
            throw new InvalidOperationException($"Can't set scalar prop value on TokenUnit");

        var typeMatch = Regex.Match(matchString, TokenUnitRegexRegister.TypeRegexTemplates[parentObject.GetType()].RenderedRegex);

        if (!typeMatch.Groups[CaptureProp.Name].Success)
            return;

        var valueToSet = CaptureProp.CapturePropType switch
        {
            CapturePropType.Enum => GetEnumMatchValue(matchString),
            CapturePropType.CapturedTextSegment => new CapturedTextSegment(matchString),
            CapturePropType.Bool => !string.IsNullOrEmpty(matchString),
        };

        CaptureProp.Prop.SetValue(parentObject, valueToSet);
    }

    void SetChildTokenUnitValue(object parentObject, string matchString)
    {
        if (!IsChildTokenUnit)
            throw new InvalidOperationException($"TokenUnit values can only be set on TokenUnit types");

        var propInstance = TokenUnitRegexRegister.InstantiateFromTypeAndMatchString(CaptureProp.UnderlyingType, matchString);
        CaptureProp.Prop.SetValue(parentObject, propInstance);
    }

    object GetEnumMatchValue(string matchString)
    {
        if (!TokenUnitRegexRegister.EnumRegexes.ContainsKey(CaptureProp.UnderlyingType))
            throw new Exception($"Enum type {CaptureProp.UnderlyingType.Name} is not registered in {nameof(TokenUnitRegexRegister)}");

        foreach (var enumMemberRegex in TokenUnitRegexRegister.EnumRegexes[CaptureProp.UnderlyingType])
            if (enumMemberRegex.Value.IsMatch(matchString))
                return enumMemberRegex.Key;

        return null;
    }
}

public enum CapturePropType
{
    Enum,
    CapturedTextSegment,
    Bool,
    TokenUnit
}

