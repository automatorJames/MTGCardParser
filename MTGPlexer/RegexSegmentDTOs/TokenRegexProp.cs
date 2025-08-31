namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also a TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record TokenRegexProp : RegexPropBase
{

    public TokenRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }

    protected override void SetRegex(RegexPropInfo captureProp)
    {
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.UnderlyingType);
        RegexString = template.RegexStringNoWordBoundaries;
    }

    public override string ToString() => base.ToString();
}

