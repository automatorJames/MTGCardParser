namespace MTGPlexer.RegexSegmentDTOs.RegexComponents;

public class EndBoundary : RegexComponentBase
{
    public string Value => @"(?<!\w)";
}
