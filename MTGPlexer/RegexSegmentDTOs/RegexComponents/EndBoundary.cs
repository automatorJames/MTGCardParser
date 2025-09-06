namespace MTGPlexer.RegexSegmentDTOs.RegexComponents;

public class StartBoundary : RegexComponentBase
{
    public string Value => @"(?<!\w)";
}

