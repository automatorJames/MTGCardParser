namespace MTGPlexer.RegexGeneration.Graph;

public class CaptureContext
{
    IReadOnlyDictionary<string, Capture[]> _captureDictionary;
    Dictionary<string, CaptureInfo> _flatTree = [];

    public CaptureInfo RootCaptureInfo { get; }
    public string SourceText { get; }
    public string FullMatch { get; }

    public CaptureContext(TokenUnitNode rootNode, Match match, string sourceText)
    {
        _captureDictionary = GetNamedGroupCaptures(match);
        SourceText = sourceText;
        FullMatch = match.Value;
        RootCaptureInfo = new(this, rootNode, match);
        _flatTree[rootNode.FullyQualifiedName] = RootCaptureInfo;
    }

    public CaptureInfo this[NamedGroupNode namedGroupNode]
    {
        get
        {
            if (!_captureDictionary.TryGetValue(namedGroupNode.FullyQualifiedName, out var allCapturesForGroup))
                // This should never happen; even if an FQN has no captures, it should still appear in the dictionary
                throw new Exception($"Name '{namedGroupNode.FullyQualifiedName}' does not appear in the dictionary");

            if (allCapturesForGroup.Length == 0)
            {
                // Equivalent to "no capture found" (not necessarily an exception condition unless the caller expects one or more captures)
                return new(this, namedGroupNode);
            }

            Debug.WriteLine(namedGroupNode.FullyQualifiedName + ": " + allCapturesForGroup[0].Value);

            var captureInfos = allCapturesForGroup.Select((x, idx) => new CaptureInfo(this, namedGroupNode, x, allCapturesForGroup.Length == 1 ? null : idx));
            var captureInfo = captureInfos.First();

            if (captureInfos.Count() > 1)
                captureInfo.Siblings.AddRange(captureInfos.Skip(1));

            _flatTree[captureInfo.FullyQualifiedName] = captureInfo;

            return captureInfo;
        }
    }

    public CaptureInfo GetCaptureTree()
    {
        var children = _flatTree.Values.Except([RootCaptureInfo]);

        foreach (var child in children)
        {
            if (!_flatTree.TryGetValue(child.ParentName, out var parentCaptureInfo))
                continue;

            parentCaptureInfo.Children.Add(child);
        }

        return RootCaptureInfo;
    }

    static Dictionary<string, Capture[]> GetNamedGroupCaptures(Match match)
    {
        if (match == null || !match.Success) return new();

        var regex = (Regex)typeof(Match)
            .GetField("_regex", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(match);

        if (regex == null) return new();

        return regex.GetGroupNames()
            .Where(name => !int.TryParse(name, out _))
            .ToDictionary(
                name => name,
                name => match.Groups[name].Captures.Cast<Capture>().ToArray()
            );
    }

    public override string ToString() => FullMatch;
}