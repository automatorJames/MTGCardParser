namespace MTGCardParser.RegexSegmentDTOs.Interfaces;

public interface IPropRegexSegment
{
    public CaptureProp CaptureProp { get; }
    public bool IsChildTokenUnit { get; }
    public bool SetValueFromMatchSpan(ITokenUnit parentToken, TextSpan matchSpan);
    public bool SetScalarPropValue(ITokenUnit parentToken, TextSpan matchSpan);
    public bool SetChildTokenUnitValue(ITokenUnit parentToken, TextSpan matchSpan);
}