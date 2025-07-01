using MTGCardParser.TokenUnits.Interfaces;

namespace MTGCardParser.RegexSegmentDTOs;

public record TokenCaptureSegment : PropSegmentBase
{
    public TokenCaptureSegment(CaptureProp captureProp) : base(captureProp)
    {
        var instanceOfPropType = (ITokenUnit)Activator.CreateInstance(captureProp.UnderlyingType);
        RegexString = instanceOfPropType.RegexTemplate.RenderedRegex;
        Regex = new Regex(RegexString);
    }
}

