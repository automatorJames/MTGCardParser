namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record TokenRegexOneOfProp : RegexPropBase
{
    public TokenRegexOneOfProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }

    protected override void SetRegex(RegexPropInfo captureProp)
    {
        RegexString = TokenTypeRegistry.GetTypeTemplate(captureProp.UnderlyingType).RegexString;
    }

    public override bool SetValueFromMatchSpan(TokenUnit parentToken, TextSpan matchSpan)
    {
        var subMatchSpan = GetPropSubMatch(matchSpan);

        if (subMatchSpan is null)
            throw new Exception($"No alternative for {nameof(TokenUnitOneOf)} type '{RegexPropInfo.UnderlyingType.Name}' matched '{matchSpan.ToStringValue()}'");

        var oneOfPropInstance = TokenUnit.InstantiateFromMatchString(RegexPropInfo.UnderlyingType, subMatchSpan.Value, parentToken, RegexPropInfo);
        RegexPropInfo.Prop.SetValue(parentToken, oneOfPropInstance);
        parentToken.AddPropertyCapture(RegexPropInfo, subMatchSpan.Value, oneOfPropInstance);
        parentToken.ChildTokens.Add(oneOfPropInstance);
        return true;
    }

    public override string ToString() => base.ToString();
}
