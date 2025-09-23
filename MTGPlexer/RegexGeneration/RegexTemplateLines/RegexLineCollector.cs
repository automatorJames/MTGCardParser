namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public class RegexLineCollector
{
    List<RegexTemplateLine> _lines = [];
    int _nextEnclosureOrdinal;
    Stack<Enclosure> _enclosureStack = [];
    Dictionary<Enclosure, int> _enclosureTerminalPropCount = [];
    BoundaryOption _boundaryOption;
    
    Dictionary<Enclosure, SpaceDisposition> _spaceIsRequiredBeforeNextElementAtLevel;

    Enclosure[] _orderedEnclosureStack =>
        _enclosureStack
            .Where(x => x is not RootEnclosure)
            .Reverse()
            .ToArray();

    public RegexLineCollector(Type topLevelType, bool neverAddSpacesAtTopLevel = false)
    {
        // an invisible top level enclosure;
        RootEnclosure rootEnclosure = new(); 

        // always track the root enclosure (makes space disposition tracking cleaner)
        _enclosureStack.Push(rootEnclosure); 

        _boundaryOption = topLevelType.GetCustomAttribute<RegexBoundaryOptionAtrribute>()?.Option ?? BoundaryOption.WholeWord;

        var topLevelSpaceDiposition = (topLevelType.IsDefined(typeof(NoSpacesAttribute)) || neverAddSpacesAtTopLevel)
            ? SpaceDisposition.NeverAddSpaceLocal
            : SpaceDisposition.DontAddSpaceBeforeNextItem;

        _spaceIsRequiredBeforeNextElementAtLevel = new Dictionary<Enclosure, SpaceDisposition> { [rootEnclosure] = topLevelSpaceDiposition };
    }

    public void OpenGroup(RegexPropInfo captureGroup = null, SpaceDisposition? spaceDisposition = null, string nameOverride = null)
    {
        AddPrecedingSpaceIfApplicable();
        Enclosure enclosure = null;

        if (captureGroup != null)
        {
            Palette palette = null;

            if (captureGroup.IsTerminal)
            {
                var currentEnclosure = _enclosureStack.Peek();
                _enclosureTerminalPropCount.TryAdd(currentEnclosure, 0);
                palette = DeterministicPalette.GetFixedRainbowPalette(_enclosureTerminalPropCount[currentEnclosure]++);
            }
            else if (TokenTypeRegistry.Palettes.TryGetValue(captureGroup.UnderlyingType, out var typePalette))
                palette = typePalette;
            else
                palette = DeterministicPalette.GetStaticPalette(new HexColor("#696969"));

            enclosure = new NamedEnclosure(_nextEnclosureOrdinal++, palette, captureGroup, nameOverride);
        }
        else
            enclosure = new Enclosure(_nextEnclosureOrdinal++);

        _enclosureStack.Push(enclosure);

        spaceDisposition ??= (captureGroup?.BaseType.IsDefined(typeof(NoSpacesAttribute)) ?? false) 
            ? SpaceDisposition.NeverAddSpaceLocal
            : SpaceDisposition.DontAddSpaceBeforeNextItem;

        _spaceIsRequiredBeforeNextElementAtLevel[enclosure] = spaceDisposition.Value;

        if (captureGroup != null)
        {
            var name = nameOverride ?? captureGroup.Name;
            _lines.Add(new NamedGroupOpen(_orderedEnclosureStack, name, captureGroup, captureGroup.FriendlyTypeName));
        }
        else
            _lines.Add(new GroupOpen(_orderedEnclosureStack));
    }

    public void CloseGroup(GroupQuantifier? quantifier = null)
    {
        if (_enclosureStack.Peek() is RootEnclosure)
            throw new Exception($"No groups are available to close");

        if (_enclosureStack.Peek() is NamedEnclosure namedEnclosure)
            _lines.Add(new NamedGroupClose(_orderedEnclosureStack, namedEnclosure.Name, quantifier));
        else
            _lines.Add(new GroupClose(_orderedEnclosureStack, quantifier));

        _enclosureStack.Pop();
    }

    public void AddTextLine(string text)
    {
        AddPrecedingSpaceIfApplicable();
        _lines.Add(new TextLine(_orderedEnclosureStack, text));
    }

    public void AddAlternatingValues(IEnumerable<string> alternatives)
    {
        var alternativeList = alternatives.ToList();

        for (int i = 0; i < alternativeList.Count; i++)
        {
            var alternative = alternativeList[i];

            var alternateValue = new AlternateValue(
                _orderedEnclosureStack,
                alternative,
                i,
                alternativeList.Count);

            _lines.Add(alternateValue);
        }
    }

    public void AddAlternatingEnumValues(EnumScalarAlternativeSet enumSet)
    {
        foreach (var enumAlternative in enumSet.EnumAlternatives)
        {
            var alternateValueEnum = new AlternateValueEnum(
                _orderedEnclosureStack,
                enumSet.ItemCount,
                enumAlternative,
                enumSet.LongestChildName);

            _lines.Add(alternateValueEnum);
        }
    }

    public void AddGroupAlternativePipe()
    {
        var path = _orderedEnclosureStack;
        _lines.Add(new GroupAlternativePipe(_orderedEnclosureStack));
    }

    void AddPrecedingSpaceIfApplicable()
    {
        // If any parent disallows spaces globally, don't add any spaces
        if (_enclosureStack.Any(x => _spaceIsRequiredBeforeNextElementAtLevel[x] == SpaceDisposition.NeverAddSpaceGlobal))
            return;

        var currentScope = _enclosureStack.Peek();
        var groupSpaceDisposition = _spaceIsRequiredBeforeNextElementAtLevel[currentScope];

        if (groupSpaceDisposition == SpaceDisposition.AddSpaceBeforeNextItem)
            _lines.Add(new SpaceLine(_orderedEnclosureStack));
        else if (groupSpaceDisposition != SpaceDisposition.NeverAddSpaceLocal)
            _spaceIsRequiredBeforeNextElementAtLevel[currentScope] = SpaceDisposition.AddSpaceBeforeNextItem;
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

        List<RegexTemplateLine> finalizedLines = [_lines[0]];

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
    NeverAddSpaceLocal,
    NeverAddSpaceGlobal,
    DontAddSpaceBeforeNextItem,
    AddSpaceBeforeNextItem,
}