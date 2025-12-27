namespace MTGPlexer.CommonDTOs;

public record TokenUnitMatch
{
    public Type Type { get; set; }
    public Match RegexMatch { get; init; }
    public SourceTextDTO SourceText { get; set; }
    public CaptureGroupPropPath CapturePath { get; init; }
    public int CaptureIndex { get; init; }
    public int AbsoluteEnd { get; init; }
    public string OverrideGroupName { get; init; }

    public TokenUnitMatch(
        Type type,
        Match regexMatch,
        SourceTextDTO sourceText = null,
        CaptureGroupPropPath capturePath = null,
        int captureIndex = 0,
        string overrideGroupName = null)
    {
        ArgumentNullException.ThrowIfNull(regexMatch);

        Type = type;
        RegexMatch = regexMatch;
        SourceText = sourceText;
        CapturePath = capturePath;
        CaptureIndex = captureIndex;
        OverrideGroupName = overrideGroupName;
        AbsoluteEnd = RegexMatch.Index + RegexMatch.Length;
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

    public Capture GetCaptureAtRelativePath(CaptureGroupPropBase captureGroup) => GetCapturesAtRelativePath(captureGroup.Name).FirstOrDefault();
    public IEnumerable<Capture> GetCapturesAtRelativePath(CaptureGroupPropBase captureGroup) => GetCapturesAtRelativePath(captureGroup.Name);
    public Capture GetCaptureAtRelativePath(params string[] relativePathParts) => GetCapturesAtRelativePath(relativePathParts).FirstOrDefault();

    /// <summary>
    /// Drills down into a Regex match following a specific path of named groups.
    /// Returns ALL captures that exist within that hierarchical path, handling quantified groups correctly.
    /// </summary>
    public IEnumerable<Capture> GetCapturesAtRelativePath(params string[] relativePathParts)
    {
        var parentParts = CapturePath.PropPathRelativeToRoot?.Split('.') ?? [];
        var absolutePathParts = parentParts.Concat(relativePathParts);

        // 1. Start with the match itself as the initial "allowed scope"
        IEnumerable<Capture> currentScopes = [RegexMatch];

        foreach (var groupName in absolutePathParts)
        {
            var targetGroup = RegexMatch.Groups[groupName];

            // Optimization: If the group was never matched anywhere, stop immediately.
            if (!targetGroup.Success) return Enumerable.Empty<Capture>();

            // 2. Collect all captures of 'groupName' that fit strictly inside ANY of the current scopes
            var nextScopes = new List<Capture>();

            // We iterate the global list of captures for this group (targetGroup.Captures)
            // and keep only those that fall within one of our active parent windows.
            foreach (Capture candidate in targetGroup.Captures)
            {
                foreach (var scope in currentScopes)
                {
                    // Check if candidate is strictly inside the scope
                    if (candidate.Index >= scope.Index &&
                       (candidate.Index + candidate.Length) <= (scope.Index + scope.Length))
                    {
                        nextScopes.Add(candidate);
                        // A capture can typically only belong to one immediate parent instance, 
                        // so we can break the inner loop once found (optimization).
                        break;
                    }
                }
            }

            // If we hit a dead end in the path, return empty
            if (nextScopes.Count == 0) return Enumerable.Empty<Capture>();

            // 3. The found children become the "scopes" for the next level in the path
            currentScopes = nextScopes;
        }

        return currentScopes;
    }

    public override string ToString() => $"Match: \"{RegexMatch.Value}\"";
}
