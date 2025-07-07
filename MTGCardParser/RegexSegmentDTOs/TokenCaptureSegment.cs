namespace MTGCardParser.RegexSegmentDTOs;

public record TokenCaptureSegment : PropSegmentBase
{
    public TokenCaptureSegment(CaptureProp captureProp) : base(captureProp)
    {
        var instanceOfPropType = (TokenUnit)Activator.CreateInstance(captureProp.UnderlyingType);
        RegexString = instanceOfPropType.GetRegexTemplate().RenderedRegexString;
        Regex = new Regex(RegexString);
    }
}

