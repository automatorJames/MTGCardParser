namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// Represents a bool property on a TokenUnit. Bool property Regexes typically check for the optional presence
/// of some matching pattern. Such properties are usually expected to have a RegexPattern attribute that defines
/// its pattern(s), but in the absence of this the normalized property name is matched.
/// </summary>
public record BoolRegexProp : RegexPropBase
{
    public BoolRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }
}

