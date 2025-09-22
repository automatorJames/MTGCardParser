namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public class RegexLineCollector
{
    List<RegexTemplateLine> _lines = [];
    int _nextEnclosureOrdinal;
    Stack<Enclosure> _enclosureStack = [];
    Dictionary<Enclosure, Palette> _terminalGroupPalettes = [];
    BoundaryOption _boundaryOption;
    
    Dictionary<Enclosure, SpaceDisposition> _spaceIsRequiredBeforeNextElementAtLevel;

    Enclosure[] _orderedEnclosureStack =>
        _enclosureStack
            .Where(x => x is not RootEnclosure)
            .Reverse()
            .ToArray();

    Palette _currentPalette => GetCurrentPaletteOrNull();

    public RegexLineCollector(Type topLevelType, bool neverAddSpacesAtTopLevel = false)
    {
        // an invisible top level enclosure;
        RootEnclosure rootEnclosure = new(); 

        // always track the root enclosure (makes space disposition tracking cleaner)
        _enclosureStack.Push(rootEnclosure); 

        _boundaryOption = topLevelType.GetCustomAttribute<RegexBoundaryOptionAtrribute>()?.Option ?? BoundaryOption.WholeWord;

        var topLevelSpaceDiposition = (topLevelType.IsDefined(typeof(NoSpacesAttribute)) || neverAddSpacesAtTopLevel)
            ? SpaceDisposition.NeverAddSpace
            : SpaceDisposition.DontAddSpaceBeforeNextItem;

        _spaceIsRequiredBeforeNextElementAtLevel = new Dictionary<Enclosure, SpaceDisposition> { [rootEnclosure] = topLevelSpaceDiposition };
    }

    public void OpenGroup(RegexPropInfo captureGroup = null, bool neverAddSpacesToGroupMembers = false, string nameOverride = null)
    {
        AddPrecedingSpaceIfApplicable();

        Enclosure enclosure = captureGroup == null 
            ? new Enclosure(_nextEnclosureOrdinal++) 
            : new NamedEnclosure(_nextEnclosureOrdinal++, captureGroup, nameOverride);

        _enclosureStack.Push(enclosure);

        if (captureGroup?.BaseType.IsDefined(typeof(NoSpacesAttribute)) ?? false)
            neverAddSpacesToGroupMembers = true;

        _spaceIsRequiredBeforeNextElementAtLevel[enclosure] = neverAddSpacesToGroupMembers 
            ? SpaceDisposition.NeverAddSpace 
            : SpaceDisposition.DontAddSpaceBeforeNextItem;

        if (captureGroup != null)
        {
            if (captureGroup.IsTerminal)
                _terminalGroupPalettes.TryAdd(enclosure, DeterministicPalette.GetFixedRainbowPalette(_terminalGroupPalettes.Count));

            _terminalGroupPalettes.TryGetValue(enclosure, out var palette);
            var name = nameOverride ?? captureGroup.Name;
            _lines.Add(new NamedGroupOpen(_orderedEnclosureStack, name, captureGroup, captureGroup.FriendlyTypeName, palette));
        }
        else
            _lines.Add(new GroupOpen(_orderedEnclosureStack));
    }

    public void CloseGroup(GroupQuantifier? quantifier = null)
    {
        if (_enclosureStack.Peek() is NamedEnclosure namedEnclosure)
            _lines.Add(new NamedGroupClose(_orderedEnclosureStack, namedEnclosure.Name, _currentPalette, quantifier));
        else
            _lines.Add(new GroupClose(_orderedEnclosureStack, _currentPalette, quantifier));

        _enclosureStack.Pop();
    }

    public void AddTextLine(string text)
    {
        AddPrecedingSpaceIfApplicable();
        _lines.Add(new TextLine(_orderedEnclosureStack, text));
    }

    public void AddAlternatingValues(IEnumerable<string> alternatives)
    {
        bool isFirstAlternation = true;
        bool isOnlyAlternation = alternatives.Count() == 1;

        foreach (var alternative in alternatives)
        {
            var alternateValue = new AlternateValue(
                _orderedEnclosureStack,
                alternative,
                GetCurrentPaletteOrNull(),
                isFirstAlternation,
                isOnlyAlternation);

            _lines.Add(alternateValue);
            isFirstAlternation = false;
        }
    }

    public void AddGroupAlternativePipe()
    {
        var path = _orderedEnclosureStack;
        _lines.Add(new GroupAlternativePipe(_orderedEnclosureStack));
    }

    void AddPrecedingSpaceIfApplicable()
    {
        var currentScope = _enclosureStack.Peek();
        var groupSpaceDisposition = _spaceIsRequiredBeforeNextElementAtLevel[currentScope];

        if (groupSpaceDisposition == SpaceDisposition.AddSpaceBeforeNextItem)
            _lines.Add(new SpaceLine(_orderedEnclosureStack));
        else if (groupSpaceDisposition != SpaceDisposition.NeverAddSpace)
            _spaceIsRequiredBeforeNextElementAtLevel[currentScope] = SpaceDisposition.AddSpaceBeforeNextItem;
    }

    Palette GetCurrentPaletteOrNull()
    {
        _terminalGroupPalettes.TryGetValue(_enclosureStack.Peek(), out var palette);
        return palette;
    }

    public Regex ExtractGroupRegex(RegexPropInfo group)
    {
        var firstGroupLine = _lines.FirstOrDefault(x => x.Enclosures.OfType<NamedEnclosure>().LastOrDefault()?.RegexPropInfo == group);
        var lastGroupLine = _lines.LastOrDefault(x => x.Enclosures.OfType<NamedEnclosure>().LastOrDefault()?.RegexPropInfo == group);

        if (firstGroupLine == null || lastGroupLine == null)
            return null;

        var firstLineIndex = _lines.IndexOf(firstGroupLine);
        var lastLineIndex = _lines.IndexOf(lastGroupLine);

        var groupLines = _lines.Skip(firstLineIndex).Take(lastLineIndex - firstLineIndex + 1).ToList();
        AddBoundaryLines(groupLines);
        var regexString = string.Join("", groupLines.Select(x => x.Regex));
        regexString = MinifyRegex(regexString);

        return new (regexString, RegexOptions.Compiled);
    }

    public GeneratedRegex Finalize()
    {
        if (!_lines.Any())
            return new GeneratedRegex([]);

        var finalizedLines = new List<RegexTemplateLine>();
        finalizedLines.Add(_lines[0]);

        for (int i = 1; i < _lines.Count; i++)
        {
            var previousLine = _lines[i - 1];
            var currentLine = _lines[i];

            bool pathChanged = currentLine.Path != previousLine.Path;

            if (pathChanged)
            {
                // Helper to classify lines into "Enter", "Exit", or "Content" events.
                // 0 = Content, 1 = Enter, 2 = Exit
                int GetEnclosureEventType(RegexTemplateLine line)
                {
                    if (line is GroupOpen or NamedGroupOpen) return 1;
                    if (line is GroupClose or NamedGroupClose) return 2;
                    return 0;
                }

                var prevEventType = GetEnclosureEventType(previousLine);
                var currentEventType = GetEnclosureEventType(currentLine);

                // Add a blank line for a path change, UNLESS it's between two consecutive
                // enclosure 'Enter' events or two consecutive 'Exit' events.
                if (prevEventType != currentEventType || prevEventType == 0)
                {
                    // Find the common parent scope for the blank line to live in.
                    int commonDepth = 0;
                    while (commonDepth < previousLine.Enclosures.Length &&
                           commonDepth < currentLine.Enclosures.Length &&
                           previousLine.Enclosures[commonDepth].Ordinal == currentLine.Enclosures[commonDepth].Ordinal)
                    {
                        commonDepth++;
                    }

                    var blankLineEnclosures = previousLine.Enclosures.Take(commonDepth).ToArray();
                    finalizedLines.Add(new BlankLine(blankLineEnclosures));
                }
            }

            finalizedLines.Add(currentLine);
        }

        AddBoundaryLines(finalizedLines);
        return new GeneratedRegex(finalizedLines);
    }

    void AddBoundaryLines(List<RegexTemplateLine> lines)
    {
        if (_boundaryOption == BoundaryOption.Omit)
            return;

        RegexTemplateLine startBoundary = _boundaryOption == BoundaryOption.WholeWord ? new NegativeLookbehindBoundary() : new StartOfLineBoundary();
        RegexTemplateLine endBoundary = _boundaryOption == BoundaryOption.WholeWord ? new NegativeLookaheadBoundary() : new EndOfLineBoundary();

        lines.Insert(0, startBoundary);
        lines.Insert(1, new BlankLine([]));
        lines.Add(new BlankLine([]));
        lines.Add(endBoundary);
    }
}

public enum SpaceDisposition
{
    NeverAddSpace,
    DontAddSpaceBeforeNextItem,
    AddSpaceBeforeNextItem,
}