namespace MTGPlexer.RegexGeneration.Graph;

public class CaptureContext
{
    IReadOnlyDictionary<string, Capture[]> _captureDictionary;

    public RootCaptureTrace RootCaptureTrace { get; }
    public string SourceText { get; }
    public string FullMatch { get; }

    public CaptureContext(TokenUnitNode rootNode, Match match, string sourceText)
    {
        _captureDictionary = GetNamedGroupCaptures(match);

        var all = _captureDictionary.Values.SelectMany(x => x);
        if (all.Any(x => x.Value == "")) Debugger.Break();

        SourceText = sourceText;
        FullMatch = match.Value;
        RootCaptureTrace = new(this, rootNode, match);
    }

    public CaptureTrace this[NamedGroupNode namedGroupNode]
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

            var captureTraces = allCapturesForGroup.Select((x, idx) => new CaptureTrace(this, namedGroupNode, x, allCapturesForGroup.Length == 1 ? null : idx));
            var captureTrace = captureTraces.First();

            if (captureTraces.Count() > 1)
                captureTrace.Siblings.AddRange(captureTraces.Skip(1));

            RootCaptureTrace.AddCaptureTrace(captureTrace);

            return captureTrace;
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

    public override string ToString() => FullMatch;
}