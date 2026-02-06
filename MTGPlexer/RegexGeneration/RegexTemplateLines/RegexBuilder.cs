using MTGPlexer.RegexGeneration.GraphNodes;

namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

/// <summary>
/// Manages the construction of a logical sequence of regular expression elements. Acts as the single interface to translate RegexSegmentBase
/// components into properly-concatenated RegexElements, and ultimately composed Regex patterns. 
/// </summary>
public class RegexBuilder
{
    RegexElementJoiner _joiner;
    int _nextEnclosureOrdinal;
    Stack<Enclosure> _enclosureStack = [];
    BoundaryOption _boundaryOption;
    RootNode _rootNode;
    Type _rootType;
    Dictionary<CaptureNode, Action> _actionsToPerformBeforeCaptureGroupOpen;
    Dictionary<CaptureNode, Action> _actionsToPerformAfterCaptureGroupClose;

    /// <summary>
    /// Gets the current stack of enclosures, with the root at the start of the array.
    /// </summary>
    Enclosure[] _orderedEnclosureStack =>
        _enclosureStack
            .Reverse()
            .ToArray();

    public RegexBuilder(RootNode rootNode)
    {
        _rootNode = rootNode;
        _rootType = rootNode.RootType;

        // an invisible top level enclosure;
        RootEnclosure rootEnclosure = new(_rootType);

        _joiner = new(_rootType);
        _enclosureStack.Push(rootEnclosure);
        _boundaryOption = _rootType.GetCustomAttribute<RegexBoundaryOptionAtrribute>()?.Option ?? BoundaryOption.None;
        AddAnonymousWrapperActionsIfNecessary();
    }

    public void AddAnonymousWrapperActionsIfNecessary()
    {
        // TokenUnitCompound alternate values must always be wrapped in ()+ to allow multiple captures
        var isTokenUnitCompound = _rootType.IsAssignableTo(typeof(TokenUnitCompound));

        // TokenUnitOneOfs with mixed content (both text and props) require wrappers around the contiguous named groups
        var isMixedTokenUnitOneOf = _rootType.IsAssignableTo(typeof(TokenUnitCompound))
            && _rootNode.Children.Any(x => x is not CaptureNode);

        if (!isTokenUnitCompound && !isMixedTokenUnitOneOf)
            return;

        GroupQuantifier? quantifier = isTokenUnitCompound ? GroupQuantifier.OneOrMore : null;

        // The prop section is guaranteed to be contiguous with text elements on the left and/or right, so
        // we can simply get the first and the last
        var captureNodes = _rootNode.Children.OfType<CaptureNode>();

        _actionsToPerformBeforeCaptureGroupOpen.Add(captureNodes.First(), () => OpenAnonymousGroup());
        _actionsToPerformAfterCaptureGroupClose.Add(captureNodes.Last(), () => CloseGroup(quantifier));
    }

    /// <summary>
    /// Opens a new regex group, which can be named or anonymous.
    /// </summary>
    /// <param name="captureGroup">The property info for the capture group, if it's a named group.</param>
    public void OpenNamedGroup(CaptureNode captureNode)
    {
        if (_actionsToPerformAfterCaptureGroupClose.TryGetValue(captureNode, out var action))
            action.Invoke();

        var enclosure = new NamedEnclosure(_nextEnclosureOrdinal++, _enclosureStack.Count, captureNode);
        _enclosureStack.Push(enclosure);
        var groupOpenElement = new NamedGroupOpen(_orderedEnclosureStack, captureNode);
        _joiner.Append(groupOpenElement);
    }

    public void OpenAnonymousGroup()
    {
        var enclosure = new Enclosure(_nextEnclosureOrdinal++, _enclosureStack.Count);
        _enclosureStack.Push(enclosure);
        var groupOpenElement = new AnonymousGroupOpen(_orderedEnclosureStack);
        _joiner.Append(groupOpenElement);
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
            quantifier ??= namedEnclosure.CaptureNode.Navigable.Proptions.HasFlag(Proptions.Optional) ? GroupQuantifier.Optional : null;
            _joiner.Append(new NamedGroupClose(_orderedEnclosureStack, namedEnclosure.Name, quantifier));
        }
        else
            _joiner.Append(new AnonymousGroupClose(_orderedEnclosureStack, quantifier));

        var closedEnclosure = _enclosureStack.Pop();

        if (closedEnclosure is NamedEnclosure closedNamedEnclosure)
        {
            if (closedNamedEnclosure.CaptureNode.Navigable.Proptions.HasFlag(Proptions.Plural))
                _joiner.Append(new AtomElement(_orderedEnclosureStack, "(s|es)?", "optional plural"));

            if (_actionsToPerformAfterCaptureGroupClose.TryGetValue(closedNamedEnclosure.CaptureNode, out var action))
                action.Invoke();
        }
    }

    /// <summary>
    /// Adds a literal text element to the regex.
    /// </summary>
    /// <param name="text">The literal text to add.</param>
    public void AddTextLine(string text, bool doNotAddPrecedingSpace = false) 
        => _joiner.Append(new TextLine(_orderedEnclosureStack, text, doNotAddPrecedingSpace: doNotAddPrecedingSpace));

    /// <summary>
    /// Adds a set of alternative string values (e.g., "a|b|c").
    /// </summary>
    /// <param name="alternatives">The collection of alternative strings.</param>
    public void AddAlternateValues(IEnumerable<string> alternatives)
        => _joiner.Append(new AlternateValueContainer(_orderedEnclosureStack, alternatives.ToList()));

    /// <summary>
    /// Adds a set of alternative enum values.
    /// </summary>
    /// <param name="enumSet">The set of enum alternates to add.</param>
    public void AddAlternateEnumValues(EnumScalarAlternateSet enumSet)
        => _joiner.Append(new AlternateValueEnumContainer(_orderedEnclosureStack, enumSet));

    /// <summary>
    /// Adds a pipe character '|' for an alternation within the current group.
    /// </summary>
    public void AddGroupAlternativePipe()
    {
        var path = _orderedEnclosureStack;
        _joiner.Append(new GroupAlternativePipe(_orderedEnclosureStack));
    }

    public void AddNegativeSpaceLookbehindBoundary()
        =>_joiner.Append(new NegativeSpaceLookbehindBoundary(_orderedEnclosureStack));

    /// <summary>
    /// Generates a fully formatted, commented, and colorized list of regex lines.
    /// </summary>
    /// <param name="synonymData">Optional data about captured synonyms to enrich the comments.</param>
    /// <returns>A list of formatted regex lines.</returns>
    public List<RegexFormattedLine> GetFormattedLines(List<PropPathSynonymSetContainer> synonymData = null)
    {
        var finalizedElements = _joiner.RegexElements.ToList();
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
        if (!_joiner.RegexElements.Any())
            return "";

        var finalizedElements = _joiner.RegexElements.ToList();

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