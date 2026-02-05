namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public class RegexElementJoiner
{
    static readonly HashSet<char> _terminalPunctuationMarks = ['.', ',', ';', ':', '!', '?', ')', ']', '}', '>', '\''];

    public List<RegexElement> RegexElements { get; } = [];
    bool _doubleQuoteIsOpen;
    HashSet<Enclosure> _enclosuresWithContent = [];
    HashSet<Enclosure> _enclosuresPendingAlternation = [];
    HashSet<Enclosure> _enclosuresContainingAtLeastOneClosedGroup = [];
    RegexElement _lastAppendedElement;
    CaptureGroupJoinStrategy _captureGroupJoinStrategy;

    public RegexElementJoiner(Type rootType)
    {
        _captureGroupJoinStrategy = rootType switch
        {
            { } t when t.IsAssignableTo(typeof(TokenUnitOneOf)) => CaptureGroupJoinStrategy.AlternateValues,
            { } t when t.IsAssignableTo(typeof(TokenUnitCompound)) => CaptureGroupJoinStrategy.CompoundValue,
            { } t when t.IsAssignableTo(typeof(TokenUnit)) => CaptureGroupJoinStrategy.ConcatenateWithSpace,
            _ => throw new Exception($"'{rootType}' is not a valid {nameof(TokenUnit)} type")
        };
    }

    public void Append(RegexElement element)
    {
        AddJoinerBeforeNextElement(element);
        AddOptionalSpaceBeforeOptionalCaptureGroup(element);
        RegexElements.Add(element);
        MarkContentEnclosureAsHavingContent(element);
        MarkEnclosureAsPendingAlternation(element);
        TrackDoubleQuoteOpenState(element);
        TrackEnclosuresContainingAtLeastOneClosedGroup(element);
        _lastAppendedElement = element;
    }

    void AddJoinerBeforeNextElement(RegexElement nextElement)
    {
        // If the any ancestor enclosure disallows spaces globally, or the parent enclosure disallows them locally, don't add a space
        if (nextElement.SpacesDisallowedGloballyOrLocally)
            return;

        // If the "do not add preceding space" flag is explicitly set on the RegexElement, honor it
        if (nextElement.DoNotAddPrecedingSpace)
            return;

        // Atom elements are surgical self-contained units added when appropriate by the RegexBuilder, and thus are intended to be appended without preceding spaces
        if (nextElement is AtomElement)
            return;

        // If the current enclosure doesn't contain any elements yet (e.g. text line or alternate value container), then this is the first, so don't add a space
        // This doesn't apply to group open elements since they represent their own "parent" and are handled below
        if (!_enclosuresWithContent.Contains(nextElement.ParentEnclosure))
            return;

        // If the current enclosure had a GroupAlternativePipe placed in it last, clear its "pending alternation" status, and don't add a space
        if (_enclosuresPendingAlternation.Contains(nextElement.ParentEnclosure))
        {
            _enclosuresPendingAlternation.Remove(nextElement.ParentEnclosure);
            return;
        }

        // If the next element is a group close, don't add a space
        if (nextElement is IGroupClose)
            return;

        // If the last regex element is a text line which should omit trailing spaces, return early.
        // This checks whether the text line ends with an opening punctuation like '(', or other conditions
        if (_lastAppendedElement is TextLine lastText && lastText.ShouldOmitSpaceAfter())
            return;

        // If the last element ends witgh a double quote, and the total number of double quotes so far is odd (open), we've just begun a quoted block, so omit the space
        if (_lastAppendedElement.Regex.EndsWith("\"") && _doubleQuoteIsOpen)
            return;

        // Handle case where next element is a text line
        if (nextElement is TextLine nextTextLine && nextTextLine.TextValue is string nextText)
        {
            // If next text begins with terminal punctuation (e.g. period, comma, etc.), don't add a space
            if (_terminalPunctuationMarks.Contains(nextText.FirstOrDefault()))
                return;

            // If next text starts with a double quote and there's a currently-open double quote pair (odd number), omit the space before it
            if (nextText.First() == '"' && _doubleQuoteIsOpen)
                return;
        }

        // If the next element is a group alternative pipe, the following element is expected to be an alternative, so don't add a space
        if (nextElement is GroupAlternativePipe)
            return;

        // If none of the conditions above were met, add a space (constructed with the same enclosures as the element to add).
        // Determine which enclosure level the space should be placed in (if any): either at the level of the element to add,
        // or if the element to add is a group open, at one level up from it.
        Enclosure[] enclosureStackToAddJoinerTo = nextElement is IGroupOpen groupOpen ? groupOpen.ParentEnclosures : nextElement.Enclosures;

        RegexElement joinerElementToAdd = _captureGroupJoinStrategy switch
        {
            CaptureGroupJoinStrategy.AlternateValues => new GroupAlternativePipe(enclosureStackToAddJoinerTo),
            CaptureGroupJoinStrategy.CompoundValue => new GroupAlternativePipe(enclosureStackToAddJoinerTo),
            CaptureGroupJoinStrategy.ConcatenateWithSpace => new SpaceLine(enclosureStackToAddJoinerTo),
            _ => new SpaceLine(enclosureStackToAddJoinerTo),
        };

        RegexElements.Add(joinerElementToAdd);
    }

    void AddOptionalSpaceBeforeOptionalCaptureGroup(RegexElement nextElement)
    {
        // If the next element is a named group open that's optional, add an optional space before it
        if (nextElement is NamedGroupOpen nextNamedGroupOpen && nextNamedGroupOpen.IsOptional)
        {
            // Determine which enclosure level the space should be placed in (if any): either at the level of the element to add,
            // or if the element to add is a group open, at one level up from it.
            Enclosure[] enclosureStackToAddJoinerTo = nextElement is IGroupOpen groupOpen ? groupOpen.ParentEnclosures : nextElement.Enclosures;
            RegexElements.Add(new SpaceLine(enclosureStackToAddJoinerTo, isOptional: true));
        }
    }

    void MarkContentEnclosureAsHavingContent(RegexElement addedElement)
    {
        // Text (text line & alternate value container) & group opens (named or unnamed) count as content
        if (addedElement is IRegexContent or IGroupOpen)
            _enclosuresWithContent.Add(addedElement.ParentEnclosure);
    }

    void MarkEnclosureAsPendingAlternation(RegexElement addedElement)
    {
        if (addedElement is GroupAlternativePipe)
            _enclosuresPendingAlternation.Add(addedElement.ParentEnclosure);
    }

    void TrackDoubleQuoteOpenState(RegexElement addedElement)
    {
        if (addedElement is TextLine textLine && textLine.TextValue is string str && str.Length > 0)
        {
            if (str.First() == '"')
                _doubleQuoteIsOpen = !_doubleQuoteIsOpen;

            if (str.Length > 1 && str.Last() == '"')
                _doubleQuoteIsOpen = !_doubleQuoteIsOpen;
        }
    }

    void TrackEnclosuresContainingAtLeastOneClosedGroup(RegexElement addedElement)
    {
        if (addedElement is IGroupClose && addedElement.ParentEnclosure is Enclosure parentEnclosure)
            _enclosuresContainingAtLeastOneClosedGroup.Add(parentEnclosure);
    }
}