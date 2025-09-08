namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public class RegexLineCollector
{
    Type _topLevelType;
    bool _addTopLevelSpaces;
    List<RegexTemplateLine> _provisionalTrailingBuffer = [];
    List<RegexTemplateLine> _lines { get; set; } = [];
    Stack<RegexPropInfo> _captureGroupStack { get; set; } = [];
    int _indentation { get; set; }

    public RegexLineCollector(Type topLevelType)
    {
        _topLevelType = topLevelType;
        _addTopLevelSpaces = !topLevelType.IsDefined(typeof(NoSpacesAttribute));
    }

    public void OpenGroup(RegexPropInfo captureGroup = null)
    {
        // Push the group name (or null if unnamed parentheses group)
        _captureGroupStack.Push(captureGroup);
        _indentation++;
        AddLine(new NamedGroupOpen(captureGroup.Name, GetFlatNamePath(), _indentation));
    }

    public void CloseGroup(bool groupIsOptional = false)
    {
        AddLine(new GroupClose(GetFlatNamePath(), _indentation, groupIsOptional));

        // Pop the current group name (or null placeholder)
        _captureGroupStack.Pop();
        _indentation--;
        AddProvisionalSpaceAndBlankIfApplicable();
    }

    public void AddTextLine(string text)
    {
        AddLine(new TextLine(text, GetFlatNamePath(), _indentation));
        AddProvisionalSpaceAndBlankIfApplicable();
    }

    public void AddAlternateValues(IEnumerable<string> alternatives)
    {
        bool isFirstAlternation = true;

        foreach (var alternative in alternatives)
        {
            var alternateValue = new AlternateValue(alternative, GetFlatNamePath(), _indentation, isFirstAlternation);
            AddLine(alternateValue);
            isFirstAlternation = false;
        }
    }

    public void AddProvisionalSpaceAndBlankIfApplicable()
    {
        var lastNamedCaptureGroup = _captureGroupStack.LastOrDefault(x => x != null);

        if (lastNamedCaptureGroup != null)
        {
            if (!lastNamedCaptureGroup.BaseType.IsDefined(typeof(NoSpacesAttribute)))
                AddProvisionalSpaceAndBlank();
        }
        else if (_addTopLevelSpaces)
            AddProvisionalSpaceAndBlank();

        // private helper
        void AddProvisionalSpaceAndBlank()
        {
            _provisionalTrailingBuffer.Add(new SpaceLine(GetFlatNamePath(), _indentation));
            _provisionalTrailingBuffer.Add(new BlankLine(GetFlatNamePath()));
        }
    }

    void AddLine(RegexTemplateLine line)
    {
        _lines.AddRange(_provisionalTrailingBuffer);
        _provisionalTrailingBuffer.Clear();
        _lines.Add(line);
    }

    public List<RegexTemplateLine> Finalize()
    {
        _lines.AddRange(_provisionalTrailingBuffer);
        _provisionalTrailingBuffer.Clear();

        if (!_topLevelType.IsDefined(typeof(NoBoundaryAttribute)))
        {
            _lines.Insert(0, new NegativeLookbehindBoundary());
            _lines.Add(new NegativeLookaheadBoundary());
        }

        return _lines;
    }

    /// <summary>
    /// Get the current dot-navigaiton name path, which exclude any null name parts (representing unnamed parentheses groups).
    /// </summary>
    string GetFlatNamePath() => string.Join(".", _captureGroupStack.Where(x => x != null).Select(x => x.Name));
}
