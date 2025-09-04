namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// This record is used for strings defined in RegexTemplate expression bodies. These strings aren't associated
/// with any TokenUnit property, but rather must be matched as part of the TokenUnit's overall Regex.
/// </summary>
public record TextSegment : RegexSegmentBase
{
    public TextSegment(string pattern)
    {
        RegexString = pattern;
    }
}

