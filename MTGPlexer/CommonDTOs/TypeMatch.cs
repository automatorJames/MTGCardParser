namespace MTGPlexer.CommonDTOs;

public record TypeMatch
{
    public Match Match { get; init; }
    public CaptureGroupPropPath CapturePath { get; init; }
    public string DistinguishingAppendix { get; init; }
    public int CaptureIndex { get; init; }
    public Capture ChildCapture { get; init; }
    public string OverrideGroupName { get; init; }

    public TypeMatch(
        Match match,
        CaptureGroupPropPath capturePath = null,
        string distinguishingAppendix = null,
        int captureIndex = 0,
        Capture childCapture = null,
        string overrideGroupName = null)
    {
        ArgumentNullException.ThrowIfNull(match);

        Match = match;
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
                return Match;

            if (Match.Groups[groupName].Success)
                return Match.Groups[groupName];

            return null;
        }
    }

    public override string ToString()
    {
        var str = $"Match: \"{Match.Value}\"";

        if (CapturePath != null || DistinguishingAppendix != null || CaptureIndex != 0 || ChildCapture != null)
            str += " (contains add'l data)";

        return str;
    }
}
