
namespace MTGCardParser;

public interface IPropRegexSegment
{
    public CaptureProp CaptureProp { get; }
    public bool IsChildTokenUnit { get; }

    public void SetValueFromMatchString(object parentObject, string matchString);
}