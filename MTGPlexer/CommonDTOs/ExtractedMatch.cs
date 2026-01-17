namespace MTGPlexer.CommonDTOs;

public record ExtractedMatch
{
    public IReadOnlyList<ExtractedCapture> Captures { get; }
    public int Index { get; }
    public int Length { get; }
    public int End { get; }
    public string Value { get; }

    public ExtractedMatch(Match match)
    {
        List<ExtractedCapture> captures = [];

        foreach (var name in GetGroupNames(match))
        {
            var capturesInGroup = match.Groups[name].Captures
                .Select((x, idx) => new ExtractedCapture(x, name, idx, match.Groups[name].Captures.Count));

            captures.AddRange(capturesInGroup);
        }

        Captures = captures;
        Index = match.Index;
        Length = match.Length;
        End = match.Index + match.Length;
        Value = match.Value;
    }

    public ExtractedCapture[] this[string groupName]
    {
        get => Captures.Where(x => x.Name == groupName).ToArray();
    }

    /// <summary>
    /// Retrieves a list of all named groups from a regular expression match.
    /// </summary>
    /// <param name="match">The match object to inspect.</param>
    /// <param name="includeMatch">If true, the output format is "{Name}: '{Value}'". If false, only the Name is returned.</param>
    /// <param name="excludeUnsuccessfulMatches">If true, groups that did not capture a value (e.g., optional groups) are excluded.</param>
    /// <param name="orderByIndex">If true, sorts successful matches by their index in the input string. If false, all returned groups are sorted alphabetically by name. Unsuccessful matches are always placed last.</param>
    /// <returns>An ordered List of strings representing the desired group information.</returns>
    static List<string> GetGroupNames(Match match, bool includeMatch = false, bool excludeUnsuccessfulMatches = true, bool orderByIndex = true)
    {
        if (match == null) throw new ArgumentNullException(nameof(match));

        var regexField = typeof(Match).GetField("_regex", BindingFlags.NonPublic | BindingFlags.Instance);

        if (regexField?.GetValue(match) is not Regex regex)
            return [];

        // Project to an intermediate anonymous type. The compiler knows the types of Name and Group.
        var groupsQuery = regex.GetGroupNames()
            .Select(name => new { Name = name, Group = match.Groups[name] })
            .Where(g => !string.IsNullOrEmpty(g.Name) && !char.IsDigit(g.Name[0]));

        if (excludeUnsuccessfulMatches)
            groupsQuery = groupsQuery.Where(g => g.Group.Success);

        // Apply the sorting logic. `var` allows the compiler to infer the IOrderedEnumerable<T> type.
        var sortedGroups = orderByIndex
            ? groupsQuery
                .OrderByDescending(g => g.Group.Success)
                .ThenBy(g => g.Group.Index)
                .ThenBy(g => g.Name)
            : groupsQuery.OrderBy(g => g.Name);

        // Now, g.Name is correctly inferred as a string at compile-time.
        if (includeMatch)
            return sortedGroups
                .Select(g => $"{g.Name}: '{g.Group.Value}'")
                .ToList();

        // This now correctly returns List<string> because g.Name is a string.
        return sortedGroups.Select(g => g.Name).ToList();
    }
}
