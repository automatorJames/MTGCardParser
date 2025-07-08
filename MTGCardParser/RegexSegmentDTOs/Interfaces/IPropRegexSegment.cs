using MTGCardParser.BaseClasses;

namespace MTGCardParser.RegexSegmentDTOs.Interfaces;

public interface IPropRegexSegment
{
    public CaptureProp CaptureProp { get; }
    public bool IsChildTokenUnit { get; }
    public bool SetValueFromMatchSpan(TokenUnit parentToken, TextSpan matchSpan);
    public bool SetScalarPropValue(TokenUnit parentToken, TextSpan matchSpan);
    public bool SetChildTokenUnitValue(TokenUnit parentToken, TextSpan matchSpan);
}