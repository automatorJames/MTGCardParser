namespace MTGPlexer.RegexGeneration.Graph;

public class CaptureContext
{
    IReadOnlyDictionary<string, Capture[]> _captureDictionary;
    Dictionary<string, CaptureInfo> _flatTree = [];
    CaptureInfo _rootCaptureInfo;
    int? _captureIndexLatch;

    /// <summary>
    /// The captures belonging to the current group that fall within the parent's scope.
    /// </summary>
    public Capture[] ScopedCaptures { get; private set; } = [];

    /// <summary>
    /// Returns just the first capture, if any, or error if multiple Captures exist. Useful in scenarios where only 
    /// one capture is expected and should be enforced as such.
    /// </summary>
    public Capture Capture
    {
        get 
        {
            if (_captureIndexLatch.HasValue)
            {
                if (_captureIndexLatch.Value >= ScopedCaptures.Length)
                    throw new IndexOutOfRangeException(nameof(_captureIndexLatch));

                var latchedValue = _captureIndexLatch.Value;
                _captureIndexLatch = null;

                return ScopedCaptures[latchedValue];
            }

            return ScopedCaptures.SingleOrDefault();
        }
    }

    public string SourceText { get; }
    public string FullMatch { get; }

    public bool Success => ScopedCaptures.Length > 0;
    public int Count => ScopedCaptures.Length;

    /// <summary>
    /// Returns the string value of the first capture, or an empty string if no captures exist.
    /// </summary>
    public string Value => Success ? ScopedCaptures[0].Value : string.Empty;

    public CaptureContext(TokenUnitNode rootNode, Match match, string sourceText)
    {
        _captureDictionary = GetNamedGroupCaptures(match);
        ScopedCaptures = [match];
        SourceText = sourceText;
        FullMatch = match.Value;
        _rootCaptureInfo = new(rootNode, match);
        _flatTree[rootNode.FullyQualifiedName] = _rootCaptureInfo;
    }

    //// Private constructor ensures controlled creation via Create() or Indexer
    //private CaptureContext(
    //    IReadOnlyDictionary<string, Capture[]> dictionary, 
    //    Capture[] captures,
    //    CaptureInfo[] captureInfos = null,
    //    bool isRoot = false, 
    //    string sourceText = null,
    //    string fullMatch = null)
    //{
    //    _dictionary = dictionary;
    //    ScopedCaptures = captures;
    //    CaptureInfos = captureInfos;
    //    _isRoot = isRoot;
    //    SourceText = sourceText;
    //    FullMatch = fullMatch;
    //}

    ///// <summary>
    ///// Entry point: Wraps a Match into a Root CaptureContext.
    ///// </summary>
    //public static CaptureContext Create(TokenUnitNode rootNode, Match match, string sourceText) =>
    //    new CaptureContext(
    //        dictionary: GetNamedGroupCaptures(match), 
    //        captures: [match], 
    //        captureInfos: [new CaptureInfo(rootNode, match)],
    //        isRoot: true, 
    //        sourceText: sourceText, 
    //        fullMatch: match.Value);

    public void ScopeToLatchedCaptureIndex(int index)
    {
        if (index >= ScopedCaptures.Length)
            throw new IndexOutOfRangeException(nameof(index));

        _captureIndexLatch = index;
    }

    public bool this[NamedGroupNode namedGroupNode]
    {
        get
        {
            if (!_captureDictionary.TryGetValue(namedGroupNode.FullyQualifiedName, out var allCapturesForGroup))
                // This should never happen; even if an FQN has no captures, it should still appear in the dictionary
                throw new Exception($"Name '{namedGroupNode.FullyQualifiedName}' does not appear in the dictionary");

            if (allCapturesForGroup.Length == 0)
            {
                // Equivalent to "no capture found", or failure mode (not necessarily an exception condition)
                ScopedCaptures = [];
                return false;
            }

            ScopedCaptures = allCapturesForGroup.Where(child =>
                ScopedCaptures.Any(parent => child.Index >= parent.Index && (child.Index + child.Length) <= (parent.Index + parent.Length)))
                .ToArray();

            Debug.WriteLine(namedGroupNode.FullyQualifiedName + ": " + allCapturesForGroup[0].Value);

            var captureInfos = allCapturesForGroup.Select((x, idx) => new CaptureInfo(namedGroupNode, x, allCapturesForGroup.Length == 1 ? null : idx));
            var captureInfo = captureInfos.First();

            if (captureInfos.Count() > 1)
                captureInfo.Siblings.AddRange(captureInfos.Skip(1));

            _flatTree[captureInfo.FullyQualifiedName] = captureInfo;

            return true;
        }
    }

    public CaptureInfo GetCaptureTree()
    {
        var children = _flatTree.Values.Except([_rootCaptureInfo]);

        foreach (var child in children)
        {
            if (!_flatTree.TryGetValue(child.ParentName, out var parentCaptureInfo))
                continue;

            parentCaptureInfo.Children.Add(child);
        }

        return _rootCaptureInfo;
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

    public override string ToString() => Value;
}