namespace MTGPlexer.RegexGeneration.Graph;

public class CaptureContext
{
    private readonly IReadOnlyDictionary<string, Capture[]> _dictionary;
    private readonly bool _isRoot;

    public CaptureInfo[] CaptureInfos { get; }

    /// <summary>
    /// The captures belonging to the current group that fall within the parent's scope.
    /// </summary>
    public Capture[] Captures { get; private set; }

    /// <summary>
    /// Returns just the first capture, if any, or error if multiple Captures exist. Useful in scenarios where only 
    /// one capture is expected and should be enforced as such.
    /// </summary>
    public Capture Capture => Captures?.SingleOrDefault();

    public string SourceText { get; }
    public string FullMatch { get; }

    public bool Success => Captures.Length > 0;
    public int Count => Captures.Length;

    /// <summary>
    /// Returns the string value of the first capture, or an empty string if no captures exist.
    /// </summary>
    public string Value => Success ? Captures[0].Value : string.Empty;

    // Private constructor ensures controlled creation via Create() or Indexer
    private CaptureContext(
        IReadOnlyDictionary<string, Capture[]> dictionary, 
        Capture[] captures,
        CaptureInfo[] captureInfos = null,
        bool isRoot = false, 
        string sourceText = null,
        string fullMatch = null)
    {
        _dictionary = dictionary;
        Captures = captures;
        CaptureInfos = captureInfos;
        _isRoot = isRoot;
        SourceText = sourceText;
        FullMatch = fullMatch;
    }

    /// <summary>
    /// Entry point: Wraps a Match into a Root CaptureContext.
    /// </summary>
    public static CaptureContext Create(Match match, string sourceText) =>
        new CaptureContext(
            GetNamedGroupCaptures(match), 
            Array.Empty<Capture>(), 
            isRoot: true, 
            sourceText: sourceText, 
            fullMatch: match.Value);

    public CaptureContext ScopeToCaptureIndex(int index)
    {
        if (index >= Captures.Length)
            throw new IndexOutOfRangeException(nameof(index));

        var filteredCapture = Captures[index];

        return new CaptureContext(_dictionary, [filteredCapture], sourceText: SourceText, fullMatch: FullMatch);
    }

    /// <summary>
    /// Fluent Indexer. 
    /// If called on Root, returns all captures for the group name.
    /// If called on a scoped context, returns captures for the name that exist physically inside the current scope.
    /// </summary>
    public CaptureContext this[NamedGroupNode namedGroupNode]
    {
        get
        {
            if (!_dictionary.TryGetValue(namedGroupNode.FullyQualifiedName, out var allCapturesForGroup))
                return new CaptureContext(_dictionary, Array.Empty<Capture>());

            var captureInfos = allCapturesForGroup
                .Select((x, idx) => new CaptureInfo(namedGroupNode, x, allCapturesForGroup.Length == 1 ? null : idx))
                .ToArray();

            // If we are root, we don't filter (provide all captures for this name)
            if (_isRoot)
                return new CaptureContext(_dictionary, allCapturesForGroup, captureInfos);

            // If we are a scoped context, only return captures that are geographically inside our current captures
            var filtered = allCapturesForGroup.Where(child =>
                Captures.Any(parent =>
                    child.Index >= parent.Index &&
                    (child.Index + child.Length) <= (parent.Index + parent.Length)))
                .ToArray();

            return new CaptureContext(_dictionary, filtered, captureInfos, sourceText: SourceText, fullMatch: FullMatch);
        }
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