namespace MTGPlexer.CommonDTOs;

public record TokenUnitMatch
{
    public Type Type { get; }
    public Match RegexMatch { get; }
    public SourceTextDTO SourceText { get; }
    public CaptureGroupPropPath CapturePath { get; }
    public int CaptureOrdinal { get; }
    public int AbsoluteEnd { get; }

    public TokenUnitMatch(
        Type type,
        Match regexMatch,
        SourceTextDTO sourceText = null,
        CaptureGroupPropPath capturePath = null,
        int captureOrdinal = 0)
    {
        ArgumentNullException.ThrowIfNull(regexMatch);

        Type = type;
        RegexMatch = regexMatch;
        SourceText = sourceText;
        CapturePath = capturePath;
        CaptureOrdinal = captureOrdinal;
        AbsoluteEnd = RegexMatch.Index + RegexMatch.Length;
    }

    /// <summary>
    /// Takes a group leaf name and constructs a fully qualified path using CapturePath. If a capture group
    /// exists in the RegexMatch by that name it is returned. If a capture ordinal is provided, this indexer
    /// validates that the named group contgains at least as many captures as the ordinal position (note: it 
    /// does not isolate and return the capture at this position, but rather the containing group).
    /// </summary>
    public Group this[string groupLeafName, int? captureOrdinal = null]
    {
        get
        {
            if (groupLeafName == null)
                throw new ArgumentNullException(nameof(groupLeafName));

            if (captureOrdinal != null && captureOrdinal.Value < 0)
                throw new ArgumentOutOfRangeException(nameof(captureOrdinal));

            var fullyQualifiedGroupName = CapturePath.GetFullyQualifiedNameFromLeaf(groupLeafName);

            if (RegexMatch.Groups[fullyQualifiedGroupName].Success)
            {
                // If a capture ordinal is provided, the group must contain at least that many captures, else we return null
                if (captureOrdinal.HasValue && RegexMatch.Groups[fullyQualifiedGroupName].Captures.Count - 1 < captureOrdinal)
                    return null;

                // Otherwise, return the named group
                return RegexMatch.Groups[fullyQualifiedGroupName];
            }

            // No group exists for the fully qualified name
            return null;
        }
    }

    public override string ToString() => $"Match: \"{RegexMatch.Value}\"";
}
