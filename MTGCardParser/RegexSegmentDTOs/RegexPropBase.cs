namespace MTGCardParser.RegexSegmentDTOs;

/// <summary>
/// The base class for all TokenUnit properties associated with some Regex pattern, including child TokenUnit properties.
/// Includes mechanisms for setting values for properties of all relevant types.
/// </summary>
public abstract record RegexPropBase : RegexSegmentBase
{
    public RegexPropInfo RegexPropInfo { get; init; }
    public bool IsBool => RegexPropInfo.RegexPropType == RegexPropType.Bool;
    public bool IsChildTokenUnit => RegexPropInfo.RegexPropType == RegexPropType.TokenUnit;
    
    public RegexPropBase(RegexPropInfo captureProp)
    {
        RegexPropInfo = captureProp;
        SetRegex(captureProp);
    }

    protected virtual void SetRegex(RegexPropInfo captureProp)
    {
        // Default implementation

        var items = captureProp.AttributePatterns.OrderByDescending(s => s.Length).ToList();
        var combinedItems = string.Join('|', items);

        if (this is BoolRegexProp boolRegexProp)
            RegexString = $@"(?<{captureProp.Name}>\s?{combinedItems}\s?)?";
        else
            RegexString = $"(?<{captureProp.Name}>{combinedItems})";

        Regex = new Regex(RegexString);
    }

    public bool SetValueFromMatchSpan(TokenUnit parentToken, TextSpan matchSpan)
    {
        if (IsChildTokenUnit)
            return SetChildTokenUnitValue(parentToken, matchSpan);
        else
            return SetScalarPropValue(parentToken, matchSpan);       
    }

    public bool SetScalarPropValue(TokenUnit parentToken, TextSpan matchSpan)
    {
        if (IsChildTokenUnit)
            throw new InvalidOperationException($"Can't set scalar prop value on TokenUnit");

        var subMatchSpan = GetGroupSubMatch(parentToken, matchSpan);

        if (subMatchSpan is null)
            return false;

        var subMatchText = subMatchSpan.Value.ToStringValue();
        var valueToSet = RegexPropInfo.RegexPropType switch
        {
            RegexPropType.Enum => GetEnumMatchValue(subMatchText),
            RegexPropType.Placeholder => new PlaceholderCapture(subMatchText),
            RegexPropType.Bool => !string.IsNullOrEmpty(subMatchText),
        };

        RegexPropInfo.Prop.SetValue(parentToken, valueToSet);
        parentToken.PropMatches[RegexPropInfo] = subMatchSpan.Value;

        return true;
    }

    public bool SetChildTokenUnitValue(TokenUnit parentToken, TextSpan matchSpan)
    {
        if (!IsChildTokenUnit)
            throw new InvalidOperationException($"TokenUnit values can only be set on TokenUnit types");

        var subMatchSpan = GetPropTypeSubMatch(matchSpan);

        if (subMatchSpan is null)
            return false;

        var propInstance = TokenUnit.InstantiateFromMatchString(RegexPropInfo.UnderlyingType, subMatchSpan.Value, parentToken);

        if (propInstance is null)
            throw new Exception($"Failed to instantiate {RegexPropInfo.UnderlyingType.Name} from match string {matchSpan.ToStringValue()}");

        RegexPropInfo.Prop.SetValue(parentToken, propInstance);
        parentToken.PropMatches[RegexPropInfo] = subMatchSpan.Value;
        parentToken.ChildTokens.Add(propInstance);
        return true;
    }

    object GetEnumMatchValue(string matchString)
    {
        if (!TokenClassRegistry.EnumRegexes.ContainsKey(RegexPropInfo.UnderlyingType))
            throw new Exception($"Enum type {RegexPropInfo.UnderlyingType.Name} is not registered in {nameof(TokenClassRegistry)}");

        foreach (var enumMemberRegex in TokenClassRegistry.EnumRegexes[RegexPropInfo.UnderlyingType])
            if (enumMemberRegex.Value.IsMatch(matchString))
                return enumMemberRegex.Key;

        return null;
    }

    TextSpan? GetGroupSubMatch(TokenUnit parentToken, TextSpan matchSpanToCheck)
    {
        var matchText = matchSpanToCheck.ToStringValue();
        var regex = TokenClassRegistry.TokenTemplates[parentToken.GetType()].RenderedRegexString;
        var match = Regex.Match(matchText, regex);
        var matchPropGroup = match.Groups[RegexPropInfo.Name];

        if (!matchPropGroup.Success)
            return null;

        var newCombinedIndex = matchSpanToCheck.Position.Absolute + matchPropGroup.Index;
        var newPosition = new Position(newCombinedIndex, matchSpanToCheck.Position.Line, newCombinedIndex + 1);
        return new TextSpan(matchSpanToCheck.Source, newPosition, matchPropGroup.Length);
    }

    TextSpan? GetPropTypeSubMatch(TextSpan matchSpanToCheck)
    {
        var regex = TokenClassRegistry.TokenTemplates[RegexPropInfo.UnderlyingType].Regex;
        var match = regex.Match(matchSpanToCheck.ToStringValue());
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

public enum RegexPropType
{
    Enum,
    Placeholder,
    Bool,
    DistilledValue,
    TokenUnit
}

