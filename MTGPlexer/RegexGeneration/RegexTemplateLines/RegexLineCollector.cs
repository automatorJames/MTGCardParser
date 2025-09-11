namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public class RegexLineCollector
{
    Type _topLevelType;
    int _nextUnnamedCaptureGroupId;
    List<RegexTemplateLine> _lines = [];
    Stack<object> _captureGroupStack = [];
    Dictionary<RegexPropInfo, DeterministicPalette> _terminalGroupPalettes = [];
    int _indentation;

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
            _lines.Add(new NamedGroupOpen(captureGroup.Name, GetFlatNamePath(), _indentation, captureGroup.FriendlyTypeName, palette));
        }
        else
            _lines.Add(new GroupOpen(GetFlatNamePath(), _indentation));

        _indentation++;
    }

    public void CloseGroup(GroupQuantifier? quantifier = null)
    {
        _indentation--;
        var path = GetFlatNamePath();
        var groupName = GetCurrentNamedGroupOrNull()?.Name;
        _lines.Add(new GroupClose(path, _indentation, GetCurrentPaletteOrNull(), groupName, quantifier));

        // Pop the current group name (or null placeholder)
        _captureGroupStack.Pop();
    }

    public void AddTextLine(string text)
    {
        AddPrecedingSpaceIfApplicable();
        _lines.Add(new TextLine(text, GetFlatNamePath(), _indentation));
    }

    public void AddAlternatiingValues(IEnumerable<string> alternatives)
    {
        bool isFirstAlternation = true;
        bool isOnlyAlternation = alternatives.Count() == 1;

        foreach (var alternative in alternatives)
        {
            var alternateValue = new AlternateValue(
                alternative,
                GetFlatNamePath(),
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
        var path = GetFlatNamePath();
        _lines.Add(new GroupAlternativePipe(path, _indentation));
    }

    void AddPrecedingSpaceIfApplicable()
    {
        var currentScopeKey = _captureGroupStack.Any() ? _captureGroupStack.Peek() : -1;

        var groupSpaceDisposition = _spaceIsRequiredBeforeNextElementAtLevel[currentScopeKey];

        if (groupSpaceDisposition == SpaceDisposition.AddSpaceBeforeNextItem)
            _lines.Add(new SpaceLine(GetFlatNamePath(), _indentation));
        else if (groupSpaceDisposition != SpaceDisposition.NeverAddSpace)
            _spaceIsRequiredBeforeNextElementAtLevel[currentScopeKey] = SpaceDisposition.AddSpaceBeforeNextItem;
    }

    /// <summary>
    /// Get the current dot-navigaiton name path, which exclude any null name parts (representing unnamed parentheses groups).
    /// </summary>
    string GetFlatNamePath() => string.Join("_", _captureGroupStack
        .OfType<RegexPropInfo>()
        .Where(x => x != null)
        .Reverse()
        .Select(x => x.Name));

    DeterministicPalette GetCurrentPaletteOrNull()
    {
        var namedGroupOrNull = GetCurrentNamedGroupOrNull();

        if (namedGroupOrNull == null)
            return null;

        _terminalGroupPalettes.TryGetValue(namedGroupOrNull, out DeterministicPalette palette);

        return palette;
    }

    RegexPropInfo GetCurrentNamedGroupOrNull()
    {
        if (!_captureGroupStack.Any()) return null;
        var group = _captureGroupStack.Peek();
        RegexPropInfo namedGroupOrNull = group is RegexPropInfo prop ? prop : null;
        return namedGroupOrNull;
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