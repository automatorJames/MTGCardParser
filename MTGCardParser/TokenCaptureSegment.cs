
namespace MTGCardParser;

public record TokenCaptureSegment : PropRegexSegmentBase
{
    public TokenCaptureSegment(CaptureProp captureProp) : base(captureProp)
    {
        var instanceOfPropType = (ITokenUnit)Activator.CreateInstance(captureProp.UnderlyingType);
        RegexString = instanceOfPropType.RegexTemplate.RenderedRegex;
        Regex = new Regex(RegexString);
    }
}

