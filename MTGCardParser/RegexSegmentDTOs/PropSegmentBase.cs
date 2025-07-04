using System.Text.RegularExpressions;

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

    public bool SetValueFromMatchSpan(ITokenUnit parentToken, TextSpan matchSpan)
    {
        if (IsChildTokenUnit)
            return SetChildTokenUnitValue(parentToken, matchSpan);
        else
            return SetScalarPropValue(parentToken, matchSpan);       
    }

    public bool SetScalarPropValue(ITokenUnit parentToken, TextSpan matchSpan)
    {
        if (IsChildTokenUnit)
            throw new InvalidOperationException($"Can't set scalar prop value on TokenUnit");

        var subMatchSpan = GetGroupSubMatch(parentToken, matchSpan);

        if (subMatchSpan is null)
            return false;

        var valueToSet = CaptureProp.CapturePropType switch
        {
            CapturePropType.Enum => GetEnumMatchValue(subMatchSpan.Value.ToStringValue()),
            CapturePropType.CapturedTextSegment => new CapturedTextSegment(subMatchSpan.Value.ToStringValue()),
            CapturePropType.Bool => !string.IsNullOrEmpty(subMatchSpan.Value.ToStringValue()),
        };

        CaptureProp.Prop.SetValue(parentToken, valueToSet);
        parentToken.PropMatches[CaptureProp] = subMatchSpan.Value;

        return true;
    }

    public bool SetChildTokenUnitValue(ITokenUnit parentToken, TextSpan matchSpan)
    {
        if (!IsChildTokenUnit)
            throw new InvalidOperationException($"TokenUnit values can only be set on TokenUnit types");

        var subMatchSpan = GetPropTypeSubMatch(matchSpan);

        if (subMatchSpan is null)
            return false;

        var propInstance = TokenUnitBase.InstantiateFromMatchString(CaptureProp.UnderlyingType, subMatchSpan.Value, parentToken);

        if (propInstance is null)
            throw new Exception($"Failed to instantiate {CaptureProp.UnderlyingType.Name} from match string {matchSpan.ToStringValue()}");

        CaptureProp.Prop.SetValue(parentToken, propInstance);
        parentToken.ChildTokens.Add(propInstance);
        return true;
    }

    object GetEnumMatchValue(string matchString)
    {
        if (!TokenClassRegistry.EnumRegexes.ContainsKey(CaptureProp.UnderlyingType))
            throw new Exception($"Enum type {CaptureProp.UnderlyingType.Name} is not registered in {nameof(TokenClassRegistry)}");

        foreach (var enumMemberRegex in TokenClassRegistry.EnumRegexes[CaptureProp.UnderlyingType])
            if (enumMemberRegex.Value.IsMatch(matchString))
                return enumMemberRegex.Key;

        return null;
    }

    TextSpan? GetGroupSubMatch(ITokenUnit parentToken, TextSpan matchSpanToCheck)
    {
        var matchText = matchSpanToCheck.ToStringValue();
        var typeMatch = Regex.Match(matchText, TokenClassRegistry.TypeRegexTemplates[parentToken.GetType()].RenderedRegexString);
        return GetTextSubSpan(matchSpanToCheck, typeMatch);
    }

    TextSpan? GetPropTypeSubMatch(TextSpan matchSpanToCheck)
    {
        var match = TokenClassRegistry.TypeRegexes[CaptureProp.UnderlyingType].Match(matchSpanToCheck.ToStringValue());
        return GetTextSubSpan(matchSpanToCheck, match);
    }

    TextSpan? GetTextSubSpan(TextSpan originalSpan, Match match)
    {
        if (match is null || !match.Success) 
            return null;

        var combinedMatchIndex = originalSpan.Position.Absolute + match.Index;
        var newPosition = new Position(combinedMatchIndex, originalSpan.Position.Line, combinedMatchIndex + 1);

        return new TextSpan(originalSpan.Source, newPosition, match.Length);
    }

}

public enum CapturePropType
{
    Enum,
    CapturedTextSegment,
    Bool,
    TokenUnit
}

