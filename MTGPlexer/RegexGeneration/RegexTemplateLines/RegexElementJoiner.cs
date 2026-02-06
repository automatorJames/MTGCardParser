namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public class RegexElementJoiner
{
    static readonly HashSet<char> _terminalPunctuationMarks = ['.', ',', ';', ':', '!', '?', ')', ']', '}', '>', '\''];
    RegexElement _lastAppendedElement => RegexElements.LastOrDefault();

    public List<RegexElement> RegexElements { get; } = [];
    bool _doubleQuoteIsOpen;
    HashSet<Enclosure> _enclosuresContainingAtLeastOneClosedGroup = [];
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
        AddSpaceAfterTextElement(element);
        AddJoinerBeforeNextElement(element);
        AddOptionalSpaceBeforeOptionalCaptureGroup(element);
        RegexElements.Add(element);
        MarkContentEnclosureAsHavingContent(element);
        MarkEnclosureAsPendingAlternation(element);
        TrackDoubleQuoteOpenState(element);
        TrackEnclosuresContainingAtLeastOneClosedGroup(element);
    }
    
    void AddSpaceAfterTextElement(RegexElement nextElement)
    {
        // Add a space after a text line before appending additional text or opening a group
        if (
            _lastAppendedElement is TextLine textLine 
            && !textLine.ShouldOmitSpaceAfter() 
            && nextElement is IGroupOpen
            && !(_lastAppendedElement.Regex.EndsWith("\"") && _doubleQuoteIsOpen)
            )
                RegexElements.Add(new SpaceLine(textLine.Enclosures));
    }

    void AddJoinerBeforeNextElement(RegexElement nextElement)
    {
        // Determine which enclosure level the joiner: either at the level of the element to add,
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