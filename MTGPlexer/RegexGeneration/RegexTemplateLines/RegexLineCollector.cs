namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public class RegexLineCollector
{
    Type _topLevelType;
    int _nextUnnamedCaptureGroupId;
    List<RegexTemplateLine> _lines = [];
    Stack<object> _captureGroupStack = [];
    Dictionary<RegexPropInfo, DeterministicPalette> _terminalGroupPalettes = [];
    int _indentation;

    string _currentNameFlatPath => string.Join("_", _captureGroupStack
        .OfType<RegexPropInfo>()
        .Where(x => x != null)
        .Reverse()
        .Select(x => x.Name));

    RegexPropInfo _currentNamedGroup => GetCurrentNamedGroupOrNull(ignoreUnnamedGroups: true);
    DeterministicPalette _currentPalette => GetCurrentPaletteOrNull();

    // The key "-1" represents the top level (i.e. the class, not within a capture group)
    Dictionary<object, SpaceDisposition> _spaceIsRequiredBeforeNextElementAtLevel;

    public RegexLineCollector(Type topLevelType, bool neverAddSpacesAtTopLevel = false)
    {
        _topLevelType = topLevelType;
        var topLevelSpaceDiposition = SpaceDisposition.DontAddSpaceBeforeNextItem;
        if (topLevelType.IsDefined(typeof(NoSpacesAttribute)) || neverAddSpacesAtTopLevel)
        {
            topLevelSpaceDiposition = SpaceDisposition.NeverAddSpace;
        }
        _spaceIsRequiredBeforeNextElementAtLevel = new Dictionary<object, SpaceDisposition> { [-1] = topLevelSpaceDiposition };
    }

    public void OpenGroup(RegexPropInfo captureGroup = null, bool neverAddSpacesToGroupMembers = false)
    {
        AddPrecedingSpaceIfApplicable();
        var groupKey = (object)captureGroup ?? _nextUnnamedCaptureGroupId++;
        _captureGroupStack.Push(groupKey);

        if (groupKey is RegexPropInfo prop && prop.BaseType.IsDefined(typeof(NoSpacesAttribute)))
            neverAddSpacesToGroupMembers = true;

        _spaceIsRequiredBeforeNextElementAtLevel[groupKey] = neverAddSpacesToGroupMembers ? SpaceDisposition.NeverAddSpace : SpaceDisposition.DontAddSpaceBeforeNextItem;

        if (captureGroup != null)
        {
            if (captureGroup.IsTerminal)
                _terminalGroupPalettes.TryAdd(captureGroup, DeterministicPalette.GetFixedRainbowPalette(_terminalGroupPalettes.Count));

            _terminalGroupPalettes.TryGetValue(captureGroup, out DeterministicPalette palette);
            _lines.Add(new NamedGroupOpen(captureGroup.Name, _currentNameFlatPath, _indentation, captureGroup.FriendlyTypeName, palette, _currentNamedGroup));
        }
        else
            _lines.Add(new GroupOpen(_currentNameFlatPath, _indentation, _currentNamedGroup));

        _indentation++;
    }

    public void CloseGroup(GroupQuantifier? quantifier = null)
    {
        _indentation--;
        var groupName = GetCurrentNamedGroupOrNull()?.Name;
        _lines.Add(new GroupClose(_currentNameFlatPath, _indentation, _currentPalette, groupName, _currentNamedGroup, quantifier));

        // Pop the current group name (or null placeholder)
        _captureGroupStack.Pop();
    }

    public void AddTextLine(string text)
    {
        AddPrecedingSpaceIfApplicable();
        _lines.Add(new TextLine(text, _currentNameFlatPath, _indentation, _currentNamedGroup));
    }

    public void AddAlternatiingValues(IEnumerable<string> alternatives)
    {
        bool isFirstAlternation = true;
        bool isOnlyAlternation = alternatives.Count() == 1;

        foreach (var alternative in alternatives)
        {
            var alternateValue = new AlternateValue(
                alternative,
                _currentNameFlatPath,
                _indentation,
                GetCurrentPaletteOrNull(),
                GetCurrentNamedGroupOrNull(),
                isFirstAlternation,
                isOnlyAlternation);

            _lines.Add(alternateValue);
            isFirstAlternation = false;
        }
    }

    public void AddGroupAlternativePipe()
    {
        var path = _currentNameFlatPath;
        _lines.Add(new GroupAlternativePipe(path, _indentation, _currentNamedGroup));
    }

    void AddPrecedingSpaceIfApplicable()
    {
        var currentScopeKey = _captureGroupStack.Any() ? _captureGroupStack.Peek() : -1;

        var groupSpaceDisposition = _spaceIsRequiredBeforeNextElementAtLevel[currentScopeKey];

        if (groupSpaceDisposition == SpaceDisposition.AddSpaceBeforeNextItem)
            _lines.Add(new SpaceLine(_currentNameFlatPath, _indentation, _currentNamedGroup));
        else if (groupSpaceDisposition != SpaceDisposition.NeverAddSpace)
            _spaceIsRequiredBeforeNextElementAtLevel[currentScopeKey] = SpaceDisposition.AddSpaceBeforeNextItem;
    }

    DeterministicPalette GetCurrentPaletteOrNull()
    {
        var namedGroupOrNull = GetCurrentNamedGroupOrNull();

        if (namedGroupOrNull == null)
            return null;

        _terminalGroupPalettes.TryGetValue(namedGroupOrNull, out DeterministicPalette palette);

        return palette;
    }

    RegexPropInfo GetCurrentNamedGroupOrNull(bool ignoreUnnamedGroups = false)
    {
        if (!_captureGroupStack.Any()) return null;

        IEnumerable<object> groupsToCheck = ignoreUnnamedGroups ?
            _captureGroupStack.Where(x => x is RegexPropInfo)
            : _captureGroupStack;

        var group = groupsToCheck.LastOrDefault();
        RegexPropInfo namedGroupOrNull = group is RegexPropInfo prop ? prop : null;
        return namedGroupOrNull;
    }

    public Regex ExtractGroupRegex(RegexPropInfo group)
    {
        var firstGroupLine = _lines.FirstOrDefault(x => x.Group == group);
        var lastGroupLine = _lines.LastOrDefault(x => x.Group == group);

        if (firstGroupLine == null || lastGroupLine == null)
            return null;

        var firstLineIndex = _lines.IndexOf(firstGroupLine);
        var lastLineIndex = _lines.IndexOf(lastGroupLine);

        var groupLines = _lines.Skip(firstLineIndex).Take(lastLineIndex - firstLineIndex + 1);
        var regexString = string.Join("", groupLines.Select(x => x.EvaluableRegex));

        return new (regexString, RegexOptions.Compiled);
    }

    public GeneratedRegex Finalize()
    {
        if (!_lines.Any())
            return new GeneratedRegex([]);

        // This new list will hold the original lines plus the new blank lines.
        var finalizedLines = new List<RegexTemplateLine>();

        // Initialize currentPath with the first line's path to avoid adding a blank line at the start.
        var currentPath = _lines[0].Path;
        finalizedLines.Add(_lines[0]);

        // Iterate through the rest of the lines to check for path changes.
        for (int i = 1; i < _lines.Count; i++)
        {
            var line = _lines[i];

            // If the path of the current line is different from the last line's path
            if (line.Path != currentPath)
            {
                // insert a blank line and update the current path
                finalizedLines.Add(new BlankLine(line.Path));
                currentPath = line.Path;
            }

            // Add the current line itself.
            finalizedLines.Add(line);
        }

        // Add word boundaries to the start and end of the entire pattern unless opted out.
        if (!_topLevelType.IsDefined(typeof(NoBoundaryAttribute)))
        {
            finalizedLines.Insert(0, new NegativeLookbehindBoundary());
            finalizedLines.Insert(1, new BlankLine(""));
            finalizedLines.Add(new BlankLine(""));
            finalizedLines.Add(new NegativeLookaheadBoundary());
        }

        // Create the final GeneratedRegex object using the fully processed list.
        return new(finalizedLines);
    }
}

public enum SpaceDisposition
{
    NeverAddSpace,
    DontAddSpaceBeforeNextItem,
    AddSpaceBeforeNextItem,
}