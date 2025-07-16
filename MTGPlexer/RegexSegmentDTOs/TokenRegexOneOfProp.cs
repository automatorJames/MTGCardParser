namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record TokenRegexOneOfProp : RegexPropBase
{
    List<TokenRegexProp> _alternativeTokenProps;

    public TokenRegexOneOfProp(RegexPropInfo captureProp) : base(captureProp)
    {
        _alternativeTokenProps = captureProp.UnderlyingType
            .GetPublicDeclaredProps()
            .Select(x => new RegexPropInfo(x))
            .Select(y => new TokenRegexProp(y))
            .ToList();

        RegexString = $"({string.Join('|', _alternativeTokenProps.Select(x => x.RegexString))})";
        Regex = new Regex(RegexString);
    }

    protected override void SetRegex(RegexPropInfo captureProp)
    {
        // Regex already set in constructor
        // This override prevents the base class overwriting it
    }

    public override bool SetValueFromMatchSpan(TokenUnit parentToken, TextSpan matchSpan)
    {
        var subMatchSpan = GetPropTypeSubMatch(matchSpan);

        if (subMatchSpan is null)
            throw new Exception($"No alternative for {nameof(TokenUnitOneOf)} type '{RegexPropInfo.UnderlyingType.Name}' matched '{matchSpan.ToStringValue()}'");

        var oneOfPropInstance = TokenUnit.InstantiateFromMatchString(RegexPropInfo.UnderlyingType, subMatchSpan.Value, parentToken);
        RegexPropInfo.Prop.SetValue(parentToken, oneOfPropInstance);
        parentToken.AddPropertyCapture(RegexPropInfo, subMatchSpan.Value, oneOfPropInstance);
        parentToken.ChildTokens.Add(oneOfPropInstance);
        return true;
    }

    public override string ToString() => base.ToString();
}