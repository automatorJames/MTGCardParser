namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

/// <summary>
/// Manages the construction of a logical sequence of regular expression elements.
/// </summary>
public class RegexBuilder
{
    RegexElementConcatenater _concatenater;
    int _nextEnclosureOrdinal;
    Stack<Enclosure> _enclosureStack = [];
    BoundaryOption _boundaryOption;

    // For convenience
    Enclosure _currentEnclosure => _enclosureStack.Count == 0 ? null : _enclosureStack.Peek();

    /// <summary>
    /// Gets the current stack of enclosures, with the root at the start of the array.
    /// </summary>
    Enclosure[] _orderedEnclosureStack =>
        _enclosureStack
            .Reverse()
            .ToArray();

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="topLevelType">The top-level type that defines the overall regex structure and attributes.</param>
    /// <param name="neverAddSpacesAtTopLevel">If true, prevents the builder from adding spaces at the root level.</param>
    public RegexBuilder(Type topLevelType)
    {
        // an invisible top level enclosure;
        RootEnclosure rootEnclosure = new(topLevelType);

        _concatenater = new();
        _enclosureStack.Push(rootEnclosure);
        _boundaryOption = topLevelType.GetCustomAttribute<RegexBoundaryOptionAtrribute>()?.Option ?? BoundaryOption.OptionalTerminalPeriod;
    }

    /// <summary>
    /// Opens a new regex group, which can be named or anonymous.
    /// </summary>
    /// <param name="captureGroup">The property info for the capture group, if it's a named group.</param>
    /// <param name="spaceDisposition">The spacing behavior for this group.</param>
    public void OpenGroup(RegexPropInfo captureGroup = null, SpaceDisposition? spaceDisposition = null)
    {
        Enclosure enclosure = null;

        if (captureGroup != null)
            enclosure = new NamedEnclosure(_nextEnclosureOrdinal++, _enclosureStack.Count, captureGroup, spaceDisposition);
        else
            enclosure = new Enclosure(_nextEnclosureOrdinal++, _enclosureStack.Count, spaceDisposition: spaceDisposition);

        // If this group is named and optional, add the space now that it's been opened
        _enclosureStack.Push(enclosure);

        if (captureGroup != null)
            _concatenater.Append(new NamedGroupOpen(_orderedEnclosureStack, captureGroup));
        else
            _concatenater.Append(new GroupOpen(_orderedEnclosureStack));
    }

    /// <summary>
    /// Closes the current regex group.
    /// </summary>
    /// <param name="quantifier">An optional quantifier to apply to the closed group.</param>
    public void CloseGroup(GroupQuantifier? quantifier = null)
    {
        if (_enclosureStack.Peek() is RootEnclosure)
            throw new Exception($"No groups are available to close");

        if (_enclosureStack.Peek() is NamedEnclosure namedEnclosure)
            _concatenater.Append(new NamedGroupClose(_orderedEnclosureStack, namedEnclosure.Name, quantifier));
        else
            _concatenater.Append(new GroupClose(_orderedEnclosureStack, quantifier));

        _enclosureStack.Pop();
    }

    /// <summary>
    /// Adds a literal text element to the regex.
    /// </summary>
    /// <param name="text">The literal text to add.</param>
    public void AddTextLine(string text) 
        => _concatenater.Append(new TextLine(_orderedEnclosureStack, text));

    /// <summary>
    /// Adds a set of alternative string values (e.g., "a|b|c").
    /// </summary>
    /// <param name="alternatives">The collection of alternative strings.</param>
    public void AddAlternateValues(IEnumerable<string> alternatives)
        => _concatenater.Append(new AlternateValueContainer(_orderedEnclosureStack, alternatives.ToList()));

    /// <summary>
    /// Adds a set of alternative enum values.
    /// </summary>
    /// <param name="enumSet">The set of enum alternates to add.</param>
    public void AddAlternateEnumValues(EnumScalarAlternateSet enumSet)
        => _concatenater.Append(new AlternateValueEnumContainer(_orderedEnclosureStack, enumSet));

    /// <summary>
    /// Adds a pipe character '|' for an alternation within the current group.
    /// </summary>
    public void AddGroupAlternativePipe()
    {
        var path = _orderedEnclosureStack;
        _concatenater.Append(new GroupAlternativePipe(_orderedEnclosureStack));
    }

    /// <summary>
    /// Extracts the raw regex string for a specific named group.
    /// </summary>
    /// <param name="group">The property info of the group to extract.</param>
    /// <returns>A compiled Regex object for the specified group.</returns>
    public Regex ExtractGroupRegex(RegexPropInfo group)
    {
        var firstGroupLine = _concatenater.RegexElements.FirstOrDefault(x => x.Enclosures.OfType<NamedEnclosure>().LastOrDefault()?.RegexPropInfo == group);
        var lastGroupLine = _concatenater.RegexElements.LastOrDefault(x => x.Enclosures.OfType<NamedEnclosure>().LastOrDefault()?.RegexPropInfo == group);

        if (firstGroupLine == null || lastGroupLine == null)
            return null;

        var firstLineIndex = _concatenater.RegexElements.IndexOf(firstGroupLine);
        var lastLineIndex = _concatenater.RegexElements.IndexOf(lastGroupLine);
        var groupLines = _concatenater.RegexElements.Skip(firstLineIndex).Take(lastLineIndex - firstLineIndex + 1).ToList();
        AddBoundaries(groupLines);
        var regexString = string.Join("", groupLines.Select(x => x.Regex));

        return new(regexString, RegexOptions.Compiled);
    }

    /// <summary>
    /// Generates a fully formatted, commented, and colorized list of regex lines.
    /// </summary>
    /// <param name="synonymData">Optional data about captured synonyms to enrich the comments.</param>
    /// <returns>A list of formatted regex lines.</returns>
    public List<RegexCommentedLine> GetFormattedLines(List<PropPathSynonymSetContainer> synonymData = null)
    {
        var formatter = new RegexFormatter(GetFinalizedLines(), synonymData);
        return formatter.Format();
    }

    /// <summary>
    /// Generates a minified, single-line regex string.
    /// </summary>
    /// <returns>The complete regex as a single string.</returns>
    public string GetMinified() =>
        string.Join("", GetFinalizedLines().Select(x => x.Regex)).Replace("[ ]", " ");

    /// <summary>
    /// Adds start and end boundary elements to a list of regex lines based on the builder's boundary option.
    /// </summary>
    /// <param name="lines">The list of elements to add boundaries to.</param>
    void AddBoundaries(List<RegexElement> lines)
    {
        if (_boundaryOption == BoundaryOption.None)
            return;

        RegexElement startBoundary = _boundaryOption switch
        {
            BoundaryOption.WholeWord => new NegativeLookbehindBoundary(),
            BoundaryOption.FullLine => new StartOfLineBoundary(),
            _ => null
        };

        RegexElement endBoundary = _boundaryOption switch
        {
            BoundaryOption.OptionalTerminalPeriod => new OptionalTerminalPeriod(),
            BoundaryOption.WholeWord => new NegativeLookaheadBoundary(),
            BoundaryOption.FullLine => new EndOfLineBoundary(),
            _ => null
        };

        if (startBoundary != null)
        {
            lines.Insert(0, startBoundary);
            lines.Insert(1, new BlankLine([]));
            lines.Add(new BlankLine([]));
        }

        if (endBoundary != null)
            lines.Add(endBoundary);
    }

    /// <summary>
    /// Adds boundaries, then returns the lines.
    /// </summary>
    List<RegexElement> GetFinalizedLines()
    {
        var finalizedElements = _concatenater.RegexElements.ToList();
        AddBoundaries(finalizedElements);
        return finalizedElements;
    }

    public override string ToString() => GetMinified();
}