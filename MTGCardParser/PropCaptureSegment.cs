namespace MTGCardParser;

public record PropCaptureSegment : PropRegexSegmentBase
{
    public PropCaptureSegment(CaptureProp captureProp) : base(captureProp)
    {
        SetRegex();
    }

    void SetRegex()
    {
        var items = CaptureProp.AttributePatterns.OrderByDescending(s => s.Length).ToList();
        var combinedItems = string.Join('|', items);
        var isBool = CaptureProp.CapturePropType == CapturePropType.Bool;
        RegexString = $"(?<{CaptureProp.Name}>{(isBool ? @"\s?" : "")}{combinedItems}{(isBool ? @"\s?" : "")}){(isBool ? "?" : "")}";
        Regex = new Regex(RegexString);
    }
}

