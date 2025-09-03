namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// The base class for all TokenUnit properties associated with some Regex pattern, including child TokenUnit properties.
/// Includes mechanisms for setting values for properties of all relevant types.
/// </summary>
public abstract record RegexPropBase : RegexSegmentBase
{
    public RegexPropInfo RegexPropInfo { get; init; }
    public bool IsBool => RegexPropInfo.RegexPropType == RegexPropType.Bool;
    public bool IsChildTokenUnit => RegexPropInfo.RegexPropType == RegexPropType.TokenUnit;
    public bool IsChildTokenUnitOneOf => RegexPropInfo.RegexPropType == RegexPropType.TokenUnitOneOf;
    public bool IsChildTokenUnitMany => RegexPropInfo.IsTokenUnitMany;
    
    public RegexPropBase(RegexPropInfo captureProp)
    {
        RegexPropInfo = captureProp;
        SetRegex(captureProp);
    }

    protected virtual void SetRegex(RegexPropInfo captureProp)
    {
        // Default implementation

        var items = (captureProp.Prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns ?? [captureProp.Name])
            .OrderByDescending(s => s.Length).ToList();

        var combinedItems = string.Join('|', items);

        if (this is BoolRegexProp boolRegexProp)
            RegexString = $@"(?<{captureProp.Name}>[ ]?{combinedItems}[ ]?)?";
        else
            RegexString = $"(?<{captureProp.Name}>{combinedItems})";
    }

    public virtual bool SetValueFromMatchSpan(TokenUnit parentToken, TextSpan matchSpan)
    {
        if (IsChildTokenUnit)
            return SetChildTokenUnitValue(parentToken, matchSpan);
        else
            return SetScalarPropValue(parentToken, matchSpan);       
    }

    public bool SetScalarPropValue(TokenUnit parentToken, TextSpan matchSpan)
    {
        var subMatchSpan = GetGroupSubMatch(parentToken, matchSpan);

        if (subMatchSpan is null)
            return false;

        var subMatchText = subMatchSpan.Value.ToStringValue();
        var valueToSet = RegexPropInfo.RegexPropType switch
        {
            RegexPropType.Enum => GetEnumMatchValue(subMatchText),
            RegexPropType.Placeholder => new PlaceholderCapture(subMatchText),
        };

        RegexPropInfo.Prop.SetValue(parentToken, valueToSet);
        parentToken.AddPropertyCapture(RegexPropInfo, subMatchSpan.Value, valueToSet);

        return true;
    }

    public bool SetChildTokenUnitValue(TokenUnit parentToken, TextSpan matchSpan)
    {
        var subMatchSpan = GetPropSubMatch(matchSpan);

        if (subMatchSpan is null)
            return false;

        var propInstance = TokenUnit.InstantiateFromMatchString(RegexPropInfo.UnderlyingType, subMatchSpan.Value, parentToken, RegexPropInfo);

        if (propInstance is null)
            throw new Exception($"Failed to instantiate {RegexPropInfo.UnderlyingType.Name} from match string {matchSpan.ToStringValue()}");

        RegexPropInfo.Prop.SetValue(parentToken, propInstance);
        parentToken.AddPropertyCapture(RegexPropInfo, subMatchSpan.Value, propInstance);
        parentToken.ChildTokens.Add(propInstance);
        return true;
    }

    object GetEnumMatchValue(string matchString)
    {
        if (!TokenTypeRegistry.EnumMemberRegexes.ContainsKey(RegexPropInfo.UnderlyingType))
            throw new Exception($"Enum type {RegexPropInfo.UnderlyingType.Name} is not registered in {nameof(TokenTypeRegistry)}");

        foreach (var enumMemberRegex in TokenTypeRegistry.EnumMemberRegexes[RegexPropInfo.UnderlyingType])
            if (enumMemberRegex.Value.IsMatch(matchString))
                return enumMemberRegex.Key;

        return null;
    }

    protected TextSpan? GetGroupSubMatch(TokenUnit parentToken, TextSpan matchSpanToCheck)
    {
        var matchText = matchSpanToCheck.ToStringValue();
        var regex = TokenTypeRegistry.Templates[parentToken.GetType()].RegexString;
        var match = Regex.Match(matchText, regex);
        var matchPropGroup = match.Groups[RegexPropInfo.Name];

        if (!matchPropGroup.Success)
            return null;

        var newCombinedIndex = matchSpanToCheck.Position.Absolute + matchPropGroup.Index;
        var newPosition = new Position(newCombinedIndex, matchSpanToCheck.Position.Line, newCombinedIndex + 1);
        return new TextSpan(matchSpanToCheck.Source, newPosition, matchPropGroup.Length);
    }

    protected TextSpan? GetPropSubMatch(TextSpan matchSpanToCheck)
    {
        var regex = TokenTypeRegistry.Templates[RegexPropInfo.UnderlyingType].Regex;
        var match = regex.Match(matchSpanToCheck.ToStringValue());
        return GetTextSubSpan(matchSpanToCheck, match);
    }

    /// <summary>
    /// Creates a new TextSpan that represents a sub-span within an original span,
    /// based on the location of a regex capture.
    /// </summary>
    /// <param name="originalSpan">The original, larger TextSpan.</param>
    /// <param name="capture">
    /// The Capture, Group, or Match object defining the sub-span. 
    /// If a Match is provided, it must be successful.
    /// </param>
    /// <returns>A new TextSpan for the captured substring, or null if the capture is invalid.</returns>
    protected TextSpan? GetTextSubSpan(TextSpan originalSpan, Capture capture)
    {
        if (capture is null || capture is Match match && !match.Success)
            return null;

        var combinedMatchIndex = originalSpan.Position.Absolute + capture.Index;
        var newPosition = new Position(combinedMatchIndex, originalSpan.Position.Line, combinedMatchIndex + 1);

        return new TextSpan(originalSpan.Source, newPosition, capture.Length);
    }

    public override string ToString() => base.ToString();

}

public enum RegexPropType
{
    Enum,
    Placeholder,
    Bool,
    DistilledValue,
    TokenUnit,
    TokenUnitOneOf
}

