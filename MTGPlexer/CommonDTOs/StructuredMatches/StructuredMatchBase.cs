namespace MTGPlexer.CommonDTOs.StructuredMatches;

public abstract record StructuredMatchBase
{
    public Capture Capture { get; }
    public string Value { get; }
    public abstract int AbsoluteStartInSource { get; }
    public abstract int AbsoluteEndInSource { get; }
    public int RelativeStartInParent { get; }
    public int RelativeEndInParent { get; }
    public int Length { get; }
    public abstract string SourceText { get; }

    public StructuredMatchBase(Capture capture) 
    {
        Capture = capture;
        Value = capture.Value;
        RelativeStartInParent = capture.Index;
        Length = capture.Length;
        RelativeEndInParent = RelativeStartInParent + Length;
    }

    public StructuredPropMatch GetChildMatch(CaptureGroupPropBase captureGroup)
    {
        var match = captureGroup.MatchRegex.Match(Value);

        if (!match.Success) 
            return null;

        StructuredPropMatch child = new(this, match, captureGroup.RegexPropInfo);

        return child;
    }

    public StructuredSubMatch GetChildSubCapture(CaptureGroupPropBase captureGroup, Capture subCapture)
    {
        var subMatch = captureGroup.MatchRegex.Match(subCapture.Value);

        if (!subMatch.Success)
            throw new Exception("When a subMatch is provided, it must be found in the captureGroup");

        StructuredSubMatch child = new(subMatch, this, captureGroup.RegexPropInfo);

        return child;
    }
}

