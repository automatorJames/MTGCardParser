namespace MTGPlexer.CommonDTOs;

public record TokenUnitMatch
{
    public Type Type { get; set; }
    public Match RegexMatch { get; init; }
    public SourceTextDTO SourceText { get; set; }
    public CaptureGroupPropPath CapturePath { get; init; }
    public string DistinguishingAppendix { get; init; }
    public int CaptureIndex { get; init; }
    public Capture ChildCapture { get; init; }
    public string OverrideGroupName { get; init; }

    public TokenUnitMatch(
        Type type,
        Match regexMatch,
        SourceTextDTO sourceText = null,
        CaptureGroupPropPath capturePath = null,
        string distinguishingAppendix = null,
        int captureIndex = 0,
        Capture childCapture = null,
        string overrideGroupName = null)
    {
        ArgumentNullException.ThrowIfNull(regexMatch);

        Type = type;
        RegexMatch = regexMatch;
        SourceText = sourceText;
        CapturePath = capturePath;
        DistinguishingAppendix = distinguishingAppendix;
        CaptureIndex = captureIndex;
        ChildCapture = childCapture;
        OverrideGroupName = overrideGroupName;
    }

    public Group this[string groupName]
    {
        get
        {
            if (groupName == null)
                throw new ArgumentNullException(nameof(groupName));

            if (OverrideGroupName == groupName)
                return RegexMatch;

            if (RegexMatch.Groups[groupName].Success)
                return RegexMatch.Groups[groupName];

            return null;
        }
    }

    public override string ToString()
    {
        var str = $"Match: \"{RegexMatch.Value}\"";

        if (CapturePath != null || DistinguishingAppendix != null || CaptureIndex != 0 || ChildCapture != null)
            str += " (contains add'l data)";

        return str;
    }
}
