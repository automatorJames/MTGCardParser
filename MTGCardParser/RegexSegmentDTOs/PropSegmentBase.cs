using MTGCardParser.RegexSegmentDTOs.Interfaces;

namespace MTGCardParser.RegexSegmentDTOs;

public abstract record PropSegmentBase : RegexSegmentBase, IPropRegexSegment
{
    public CaptureProp CaptureProp { get; init; }
    public bool IsBool => CaptureProp.CapturePropType == CapturePropType.Bool;
    public bool IsChildTokenUnit => CaptureProp.CapturePropType == CapturePropType.TokenUnit;
    
    public PropSegmentBase(CaptureProp captureProp)
    {
        CaptureProp = captureProp;
    }

    public bool SetValueFromMatchString(object parentObject, string matchString)
    {
        if (IsChildTokenUnit)
            return SetChildTokenUnitValue(parentObject, matchString);
        else
        {
            SetScalarPropValue(parentObject, matchString);
            return true; // Always attempt a set, doesn't matter if it matched
        }
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

    bool SetChildTokenUnitValue(object parentObject, string matchString)
    {
        if (!IsChildTokenUnit)
            throw new InvalidOperationException($"TokenUnit values can only be set on TokenUnit types");

        var propInstance = TokenUnitRegexRegister.InstantiateFromTypeAndMatchString(CaptureProp.UnderlyingType, matchString);

        if (propInstance is not null)
        {
            CaptureProp.Prop.SetValue(parentObject, propInstance);
            return true;
        }
        else
            return false;
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

