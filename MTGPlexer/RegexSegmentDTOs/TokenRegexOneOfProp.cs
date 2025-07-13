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
    }

    protected override void SetRegex(RegexPropInfo captureProp)
    {
        var tokenProps = captureProp.UnderlyingType
            .GetPublicDeclaredProps()
            .ToList();

        RegexString = "(";

        for (int i = 0; i < tokenProps.Count; i++)
        {
            var tokenProp = tokenProps[i];
            var template = TokenTypeRegistry.GetTypeTemplate(tokenProp.PropertyType);

            RegexString += template.RenderedRegexString;

            if (i < tokenProps.Count - 1)
                RegexString += "|";
        }

        RegexString += ")";
        Regex = new Regex(RegexString);
    }

    public override bool SetValueFromMatchSpan(TokenUnit parentToken, TextSpan matchSpan)
    {
        var oneOfPropInstance = TokenUnit.InstantiateFromMatchString(RegexPropInfo.UnderlyingType, matchSpan, parentToken);
        RegexPropInfo.Prop.SetValue(parentToken, oneOfPropInstance);
        parentToken.AddPropertyCapture(RegexPropInfo, matchSpan, oneOfPropInstance);
        parentToken.ChildTokens.Add(oneOfPropInstance);
        return true;
    }
}