namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

/// <summary>
/// Manages the construction of a logical sequence of regular expression elements. Acts as the single interface to translate RegexSegmentBase
/// components into properly-concatenated RegexElements, and ultimately composed Regex patterns. 
/// </summary>
public class RegexBuilder
{
    RegexElementConcatenater _concatenater;
    int _nextEnclosureOrdinal;
    Stack<Enclosure> _enclosureStack = [];
    BoundaryOption _boundaryOption;

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
    /// <param name="rootType">The top-level type that defines the name of the root enclosure.</param>
    /// <param name="neverAddSpacesAtTopLevel">If true, prevents the builder from adding spaces at the root level.</param>
    public RegexBuilder(Type rootType)
    {
        // an invisible top level enclosure;
        RootEnclosure rootEnclosure = new(rootType);

        _concatenater = new();
        _enclosureStack.Push(rootEnclosure);
        _boundaryOption = rootType.GetCustomAttribute<RegexBoundaryOptionAtrribute>()?.Option ?? BoundaryOption.None;
    }

    /// <summary>
    /// Opens a new regex group, which can be named or anonymous.
    /// </summary>
    /// <param name="captureGroup">The property info for the capture group, if it's a named group.</param>
    /// <param name="spaceDisposition">The spacing behavior for this group.</param>
    public void OpenGroup(TemplatePropInfo captureGroup = null, SpaceDisposition? spaceDisposition = null, bool isOptional = false)
    {
        Enclosure enclosure;
        isOptional |= captureGroup?.Proptions.HasFlag(Proptions.Optional) ?? false;

        if (captureGroup != null)
            enclosure = new NamedEnclosure(_nextEnclosureOrdinal++, _enclosureStack.Count, captureGroup, spaceDisposition);
        else
            enclosure = new Enclosure(_nextEnclosureOrdinal++, _enclosureStack.Count, spaceDisposition: spaceDisposition);

        _enclosureStack.Push(enclosure);

        RegexElement groupOpenElement = null;

        if (captureGroup != null)
            groupOpenElement = new NamedGroupOpen(_orderedEnclosureStack, captureGroup);
        else
            groupOpenElement = new AnonymousGroupOpen(_orderedEnclosureStack);

        _concatenater.Append(groupOpenElement);
    }

    /// <summary>
    /// Closes the current regex group.
    /// </summary>
    /// <param name="quantifier">An optional quantifier to apply to the closed group.</param>
    public void CloseGroup(GroupQuantifier? quantifier = null)
    {
        var currentEnclosure = _enclosureStack.Peek();

        if (currentEnclosure is RootEnclosure)
            throw new Exception($"No groups are available to close");

        if (currentEnclosure is NamedEnclosure namedEnclosure)
        {
            quantifier ??= namedEnclosure.TemplatePropInfo.Proptions.HasFlag(Proptions.Optional) ? GroupQuantifier.Optional : null;
            _concatenater.Append(new NamedGroupClose(_orderedEnclosureStack, namedEnclosure.Name, quantifier));
        }
        else
            _concatenater.Append(new AnonymousGroupClose(_orderedEnclosureStack, quantifier));

        var closedEnclosure = _enclosureStack.Pop();

        if (closedEnclosure is NamedEnclosure closedNamedEnclosure)
        {
            if (closedNamedEnclosure.TemplatePropInfo.Proptions.HasFlag(Proptions.Plural))
                _concatenater.Append(new AtomElement(_orderedEnclosureStack, "(s|es)?", "optional plural"));
        }
    }

    /// <summary>
    /// Adds a literal text element to the regex.
    /// </summary>
    /// <param name="text">The literal text to add.</param>
    public void AddTextLine(string text, bool doNotAddPrecedingSpace = false) 
        => _concatenater.Append(new TextLine(_orderedEnclosureStack, text, doNotAddPrecedingSpace: doNotAddPrecedingSpace));

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

    public void AddNegativeSpaceLookbehindBoundary()
        =>_concatenater.Append(new NegativeSpaceLookbehindBoundary(_orderedEnclosureStack));

    /// <summary>
    /// Extracts the raw regex string for a specific named group.
    /// </summary>
    /// <param name="group">The property info of the group to extract.</param>
    /// <returns>A compiled Regex object for the specified group.</returns>
    public Regex ExtractGroupRegex(TemplatePropInfo group)
    {
        var firstGroupLine = _concatenater.RegexElements.FirstOrDefault(x => x.Enclosures.OfType<NamedEnclosure>().LastOrDefault()?.TemplatePropInfo == group);
        var lastGroupLine = _concatenater.RegexElements.LastOrDefault(x => x.Enclosures.OfType<NamedEnclosure>().LastOrDefault()?.TemplatePropInfo == group);

        if (firstGroupLine == null || lastGroupLine == null)
            return null;

        var firstLineIndex = _concatenater.RegexElements.IndexOf(firstGroupLine);
        var lastLineIndex = _concatenater.RegexElements.IndexOf(lastGroupLine);

        var groupLines = _concatenater.RegexElements.Skip(firstLineIndex).Take(lastLineIndex - firstLineIndex + 1).ToList();
        AddBoundaryLines(groupLines);
        var regexString = string.Join("", groupLines.Select(x => x.Regex));

        return new(regexString, RegexOptions.Compiled);
    }

    /// <summary>
    /// Generates a fully formatted, commented, and colorized list of regex lines.
    /// </summary>
    /// <param name="synonymData">Optional data about captured synonyms to enrich the comments.</param>
    /// <returns>A list of formatted regex lines.</returns>
    public List<RegexFormattedLine> GetFormattedLines(List<PropPathSynonymSetContainer> synonymData = null)
    {
        var finalizedElements = _concatenater.RegexElements.ToList();
        AddBoundaryLines(finalizedElements);
        var formatter = new RegexFormatter(finalizedElements, synonymData);
        return formatter.Format();
    }

    /// <summary>
    /// Generates a minified, single-line regex string.
    /// </summary>
    /// <returns>The complete regex as a single string.</returns>
    public string GetMinified(bool addBoundaries = true)
    {
        if (!_concatenater.RegexElements.Any())
            return "";

        var finalizedElements = _concatenater.RegexElements.ToList();

        if (addBoundaries)
            AddBoundaryLines(finalizedElements);

        return string.Join("", finalizedElements.Select(x => x.Regex)).Replace("[ ]", " ");
    }

    /// <summary>
    /// Adds start and end boundary elements to a list of regex lines based on the builder's boundary option.
    /// </summary>
    /// <param name="lines">The list of elements to add boundaries to.</param>
    void AddBoundaryLines(List<RegexElement> lines)
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
            BoundaryOption.WholeWord => new NegativeLookaheadBoundary(),
            BoundaryOption.FullLine => new EndOfLineBoundary(),
            _ => null
        };

        if (startBoundary != null)
        {
            lines.Insert(0, startBoundary);
            lines.Insert(1, new BlankLine([]));
        }

        if (endBoundary != null)
        {
            lines.Add(new BlankLine([]));
            lines.Add(endBoundary);
        }
    }

    public BuiltRegex GetBuiltRegex()
    {
        var regexString = GetMinified();
        Regex regex = new(regexString, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        var lines = GetFormattedLines();

        return new(regexString, regex, lines);
    }

    public override string ToString() => GetMinified(addBoundaries: false);
}