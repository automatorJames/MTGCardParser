namespace MTGPlexer.CommonDTOs;

public record TypeMatch
{
    public Type Type { get; set; }
    public Match Match { get; init; }
    public SourceTextDTO SourceText { get; set; }
    public CaptureGroupPropPath CapturePath { get; init; }
    public string DistinguishingAppendix { get; init; }
    public int CaptureIndex { get; init; }
    public Capture ChildCapture { get; init; }
    public string OverrideGroupName { get; init; }

    public TypeMatch(
        Type type,
        Match match,
        SourceTextDTO sourceText = null,
        CaptureGroupPropPath capturePath = null,
        string distinguishingAppendix = null,
        int captureIndex = 0,
        Capture childCapture = null,
        string overrideGroupName = null)
    {
        ArgumentNullException.ThrowIfNull(match);

        Type = type;
        Match = match;
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
