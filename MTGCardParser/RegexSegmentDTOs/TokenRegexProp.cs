namespace MTGCardParser.RegexSegmentDTOs;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record TokenRegexProp : RegexPropBase
{
    public TokenRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
        var instanceOfPropType = (TokenUnit)Activator.CreateInstance(captureProp.UnderlyingType);
        RegexString = instanceOfPropType.GetRegexTemplate().RenderedRegexString;
        Regex = new Regex(RegexString);
    }
}

