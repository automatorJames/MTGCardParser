using static System.Net.Mime.MediaTypeNames;

namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

/// <summary>
/// Manages the construction of a logical sequence of regular expression elements.
/// </summary>
public class RegexBuilder
{
    List<RegexElement> _regexElements = [];
    int _nextEnclosureOrdinal;
    Stack<Enclosure> _enclosureStack = [];
    Dictionary<Enclosure, int> _enclosureTerminalPropCount = [];
    Dictionary<Enclosure, char> _lastCharPerEnclosure = [];
    BoundaryOption _boundaryOption;
    static readonly HashSet<char> _openingPunctuationMarks = ['(', '[', '{', '<'];
    static readonly HashSet<char> _terminalPunctuationMarks = ['.', ',', ';', ':', '!', '?', ')', ']', '}', '>'];
    bool _doubleQuoteIsOpen;

    // For convenience
    Enclosure _currentEnclosure => _enclosureStack.Count == 0 ? null : _enclosureStack.Peek();

    Dictionary<Enclosure, SpaceDisposition> _spaceIsRequiredBeforeNextElementAtLevel;

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
    public RegexBuilder(Type topLevelType, bool neverAddSpacesAtTopLevel = false)
    {
        // an invisible top level enclosure;
        RootEnclosure rootEnclosure = new(topLevelType.Name);

        // always track the root enclosure (makes space disposition tracking cleaner)
        _enclosureStack.Push(rootEnclosure);

        _boundaryOption = topLevelType.GetCustomAttribute<RegexBoundaryOptionAtrribute>()?.Option ?? BoundaryOption.WholeWord;

        var topLevelSpaceDiposition = (topLevelType.IsDefined(typeof(NoSpacesAttribute)) || neverAddSpacesAtTopLevel)
            ? SpaceDisposition.NeverAddSpaceLocal
            : SpaceDisposition.DontAddSpaceBeforeNextItem;

        _spaceIsRequiredBeforeNextElementAtLevel = new Dictionary<Enclosure, SpaceDisposition> { [rootEnclosure] = topLevelSpaceDiposition };
    }

    /// <summary>
    /// Opens a new regex group, which can be named or anonymous.
    /// </summary>
    /// <param name="captureGroup">The property info for the capture group, if it's a named group.</param>
    /// <param name="spaceDisposition">The spacing behavior for this group.</param>
    public void OpenGroup(RegexPropInfo captureGroup = null, SpaceDisposition? spaceDisposition = null)
    {
        bool groupIsNamedAndOptional = captureGroup?.Prop.IsDefined(typeof(OptionalComponentAttribute)) ?? false;

        // If this group is named and optional, don't add a space before it
        if (!groupIsNamedAndOptional)
            AddPrecedingSpaceIfApplicable();

        Enclosure enclosure = null;

        if (captureGroup != null)
        {
            HexPalette palette = null;

            if (captureGroup.IsTerminal)
            {
                _enclosureTerminalPropCount.TryAdd(_currentEnclosure, 0);
                palette = DeterministicPalette.GetFixedRainbowPalette(_enclosureTerminalPropCount[_currentEnclosure]++);
            }
            else if (TokenTypeRegistry.Palettes.TryGetValue(captureGroup.UnderlyingType, out var typePalette))
                palette = typePalette;
            else
                palette = DeterministicPalette.GetStaticPalette(new HexColor("#696969"));

            enclosure = new NamedEnclosure(_nextEnclosureOrdinal++, palette, captureGroup);
        }
        else
            enclosure = new Enclosure(_nextEnclosureOrdinal++);

        // If this group is named and optional, add the space now that it's been opened
        _enclosureStack.Push(enclosure);

        if (spaceDisposition == null)
        {
            if (captureGroup != null && captureGroup.BaseType.IsDefined(typeof(NoSpacesAttribute)))
                spaceDisposition = SpaceDisposition.NeverAddSpaceLocal;
            else
                spaceDisposition = SpaceDisposition.DontAddSpaceBeforeNextItem; // the default state
        }

        _spaceIsRequiredBeforeNextElementAtLevel[enclosure] = spaceDisposition.Value;

        if (captureGroup != null)
        {
            _regexElements.Add(new NamedGroupOpen(_orderedEnclosureStack, captureGroup));

            // If this optional component is not the first regex element, add a space within the beginning of the capture group
            if (groupIsNamedAndOptional && _regexElements.Count > 1)
                _regexElements.Add(new SpaceLine(_orderedEnclosureStack));
        }
        else
            _regexElements.Add(new GroupOpen(_orderedEnclosureStack));
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
            _regexElements.Add(new NamedGroupClose(_orderedEnclosureStack, namedEnclosure.Name, quantifier));
        else
            _regexElements.Add(new GroupClose(_orderedEnclosureStack, quantifier));

        _enclosureStack.Pop();
    }

    /// <summary>
    /// Adds a literal text element to the regex.
    /// </summary>
    /// <param name="text">The literal text to add.</param>
    public void AddTextLine(string text, bool doNotAddFollowingSpace = false)
    {
        // If the last char of the previous content at this level is an opening punctuation, or the
        // first char of the new content is a closing punctuation, don't add a space before the new content.
        bool shouldNotAddSpaceBeforeNextItem =
             _lastCharPerEnclosure.TryGetValue(_currentEnclosure, out var lastChar) && _openingPunctuationMarks.Contains(lastChar)
            || _terminalPunctuationMarks.Contains(text.FirstOrDefault());

        if (shouldNotAddSpaceBeforeNextItem)
            _spaceIsRequiredBeforeNextElementAtLevel[_currentEnclosure] = SpaceDisposition.DontAddSpaceBeforeNextItem;

        AddPrecedingSpaceIfApplicable();
        _regexElements.Add(new TextLine(_orderedEnclosureStack, text));
        _lastCharPerEnclosure[_currentEnclosure] = text.LastOrDefault();
        TrackDoubleQuoteOpenState(text);

        if (doNotAddFollowingSpace)
            _spaceIsRequiredBeforeNextElementAtLevel[_currentEnclosure] = SpaceDisposition.DontAddSpaceBeforeNextItem;
    }

    /// <summary>
    /// Adds a set of alternative string values (e.g., "a|b|c").
    /// </summary>
    /// <param name="alternatives">The collection of alternative strings.</param>
    public void AddAlternateValues(IEnumerable<string> alternatives)
        => _regexElements.Add(new AlternateValueContainer(_orderedEnclosureStack, alternatives.ToList()));

    /// <summary>
    /// Adds a set of alternative enum values.
    /// </summary>
    /// <param name="enumSet">The set of enum alternates to add.</param>
    public void AddAlternateEnumValues(EnumScalarAlternateSet enumSet)
        => _regexElements.Add(new AlternateValueEnumContainer(_orderedEnclosureStack, enumSet));

    /// <summary>
    /// Adds a pipe character '|' for an alternation within the current group.
    /// </summary>
    public void AddGroupAlternativePipe()
    {
        var path = _orderedEnclosureStack;
        _regexElements.Add(new GroupAlternativePipe(_orderedEnclosureStack));
    }

    /// <summary>
    /// Adds a space element if the current group's spacing rules require it.
    /// </summary>
    void AddPrecedingSpaceIfApplicable()
    {
        // If the current enclosure or any parent enclosure disallows spaces globally, don't add any spaces
        if (_enclosureStack.Any(x => _spaceIsRequiredBeforeNextElementAtLevel[x] == SpaceDisposition.NeverAddSpaceGlobal))
            return;
        
        var spaceDisposition = _spaceIsRequiredBeforeNextElementAtLevel[_currentEnclosure];

        if (spaceDisposition == SpaceDisposition.AddSpaceBeforeNextItem)
            _regexElements.Add(new SpaceLine(_orderedEnclosureStack));
        else if (spaceDisposition != SpaceDisposition.NeverAddSpaceLocal || _doubleQuoteIsOpen)
            _spaceIsRequiredBeforeNextElementAtLevel[_currentEnclosure] = SpaceDisposition.AddSpaceBeforeNextItem;
    }

    void TrackDoubleQuoteOpenState(string str)
    {
        for (int i = 0; i < str.Length; i++)
            if (str[i] == '"')
                _doubleQuoteIsOpen = !_doubleQuoteIsOpen;
    }

    /// <summary>
    /// Extracts the raw regex string for a specific named group.
    /// </summary>
    /// <param name="group">The property info of the group to extract.</param>
    /// <returns>A compiled Regex object for the specified group.</returns>
    public Regex ExtractGroupRegex(RegexPropInfo group)
    {
        var firstGroupLine = _regexElements.FirstOrDefault(x => x.Enclosures.OfType<NamedEnclosure>().LastOrDefault()?.RegexPropInfo == group);
        var lastGroupLine = _regexElements.LastOrDefault(x => x.Enclosures.OfType<NamedEnclosure>().LastOrDefault()?.RegexPropInfo == group);

        if (firstGroupLine == null || lastGroupLine == null)
            return null;

        var firstLineIndex = _regexElements.IndexOf(firstGroupLine);
        var lastLineIndex = _regexElements.IndexOf(lastGroupLine);

        var groupLines = _regexElements.Skip(firstLineIndex).Take(lastLineIndex - firstLineIndex + 1).ToList();
        AddBoundaryLines(groupLines);
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
        var formatter = new RegexFormatter();
        return formatter.Format(_regexElements, _boundaryOption, synonymData);
    }

    /// <summary>
    /// Generates a minified, single-line regex string.
    /// </summary>
    /// <returns>The complete regex as a single string.</returns>
    public string GetMinified(bool addBoundaries = true)
    {
        if (!_regexElements.Any())
            return "";

        var finalizedElements = _regexElements.ToList();

        if (addBoundaries)
            AddBoundaryLines(finalizedElements);

        return string.Join("", finalizedElements.Select(x => x.Regex)).Replace("[ ]", " ");
    }

    /// <summary>
    /// Adds start and end boundary elements to a list of regex lines based on the builder's boundary option.
    /// </summary>
    /// <param name="lines">The list of elements to add boundaries to.</param>
    private void AddBoundaryLines(List<RegexElement> lines)
    {
        if (_boundaryOption == BoundaryOption.Omit)
            return;

        RegexElement startBoundary = _boundaryOption == BoundaryOption.WholeWord ? new NegativeLookbehindBoundary() : new StartOfLineBoundary();
        RegexElement endBoundary = _boundaryOption == BoundaryOption.WholeWord ? new NegativeLookaheadBoundary() : new EndOfLineBoundary();

        lines.Insert(0, startBoundary);
        lines.Insert(1, new BlankLine([]));
        lines.Add(new BlankLine([]));
        lines.Add(endBoundary);
    }

    public override string ToString() => GetMinified(addBoundaries: false);
}