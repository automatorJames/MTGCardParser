namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public class CaptureDictionary
{
    private readonly Dictionary<string, Capture[]> _dictionary;

    public CaptureDictionary(Match match)
    {
        _dictionary = GetNamedGroupCaptures(match);
    }

    /// <summary>
    /// Indexer to access captures by the group name.
    /// </summary>
    public Capture[] this[string groupName]
    {
        get
        {
            return _dictionary.TryGetValue(groupName, out var captures)
                ? captures
                : Array.Empty<Capture>();
        }
    }

    /// <summary>
    /// Returns a string representation of all named groups and their captured values.
    /// Format: "GroupName: [Value1, Value2]"
    /// </summary>
    public override string ToString()
    {
        if (_dictionary.Count == 0) return "{ }";

        var lines = _dictionary.Select(kvp =>
        {
            var values = string.Join(", ", kvp.Value.Select(c => $"\"{c.Value}\""));
            return $"{kvp.Key}: [{values}]";
        });

        return "{ " + string.Join("; ", lines) + " }";
    }

    /// <summary>
    /// Extracts all named groups and their captures from a Match object.
    /// Excludes groups with numeric names (0, 1, 2...).
    /// </summary>
    private static Dictionary<string, Capture[]> GetNamedGroupCaptures(Match match)
    {
        if (match == null || !match.Success)
            return new Dictionary<string, Capture[]>();

        // Reach into the Match object to get the Regex instance that created it
        var regexField = typeof(Match).GetField("_regex", BindingFlags.NonPublic | BindingFlags.Instance);
        var regex = (Regex)regexField?.GetValue(match);

        if (regex == null)
            return new Dictionary<string, Capture[]>();

        // Filter out automatic numbered groups (e.g., "0", "1")
        var namedGroups = regex.GetGroupNames()
            .Where(name => !int.TryParse(name, out _));

        return namedGroups.ToDictionary(
            name => name,
            name => match.Groups[name].Captures.Cast<Capture>().ToArray()
        );
    }
}