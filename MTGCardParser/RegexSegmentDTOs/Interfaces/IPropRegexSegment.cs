namespace MTGCardParser.RegexSegmentDTOs.Interfaces;

public interface IPropRegexSegment
{
    public CaptureProp CaptureProp { get; }
    public bool IsChildTokenUnit { get; }

    public bool SetValueFromMatchString(object parentObject, string matchString);
}