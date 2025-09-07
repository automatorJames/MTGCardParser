namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// The base class for all TokenUnit properties with a name Regex capture group whose pattern, is 
/// associated with some property, including child TokenUnit properties. Includes mechanisms for 
/// setting values for properties of all relevant types.
/// </summary>
public abstract class CaptureGroupPropBase : RegexSegmentBase
{
    public string Name { get; init; }
    public string[] ParentNamePath { get; init; }
    public RegexPropInfo RegexPropInfo { get; init; }
    public bool IsOptional { get; init; }
    public bool IsChildTokenUnit { get; init; }
    public List<AlternateValue> CaptureAlternatives { get; protected set; }
    public string CaptureAlternativesString { get; protected set; }
    
    public CaptureGroupPropBase(RegexPropInfo captureProp)
    {
        Name = captureProp.Name;
        RegexPropInfo = captureProp;
        IsChildTokenUnit = RegexPropInfo.RegexPropType == RegexPropType.Bool;
        IsChildTokenUnit = RegexPropInfo.RegexPropType == RegexPropType.TokenUnit;
        SetRegex(captureProp);
    }

    protected virtual void SetRegex(RegexPropInfo captureProp)
    {
        // Default implementation

        CaptureAlternatives = (captureProp.Prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns ?? [captureProp.Name])
            .OrderByDescending(s => s.Length)
            .Select(x => new AlternateValue(x))
            .ToList();

        CaptureAlternativesString = string.Join('|', CaptureAlternatives);
        RegexString = $"(?<{captureProp.Name}>{CaptureAlternativesString})";
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

        parentToken.SetPropertyCapture(RegexPropInfo, subMatchSpan.Value, valueToSet);

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

        parentToken.SetPropertyCapture(RegexPropInfo, subMatchSpan.Value, propInstance);
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

